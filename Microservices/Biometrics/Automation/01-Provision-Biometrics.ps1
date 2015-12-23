<# 
.Synopsis 
    This PowerShell script provisions the Home Biomedical Azure application
.Description 
    This PowerShell script provisions the Home Biomedical Azure application
.Notes 
    File Name  : Provision.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Provision-Biometrics.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Subscription
    Example:  MySubscription
.Parameter AzureLocation
    Example:  East US
.Parameter Prefix
    Example:  looksfamiliar
.Parameter Suffix
    Example:  test
.Inputs
    The [Repo] parameter is the path to the Git Repo
    The [Subscription] parameter is the name of the client Azure subscription.
    The [AzureLocation] parameter is the name of the Azure Region/Location to host the Virtual Machines for this subscription.
    The [Prefix] parameter is the common prefix that will be used to name resources
    The [Suffix] parameter is one of 'dev', 'test' or 'prod'
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The path to the Git Repo.")]
    [string]$Repo,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The name of the Azure Subscription for which you've imported a *.publishingsettings file.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The name of the Azure Region/Location: East US, Central US, West US.")]
    [string]$AzureLocation,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The common prefix for resources naming: looksfamiliar")]
    [string]$Prefix,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="The suffix which is one of 'dev', 'test' or 'prod'")]
    [string]$Suffix

)

#######################################################################################
# I M P O R T S
#######################################################################################

$UpdateConfig = $Repo + "\Automation\Common\Invoke-UpdateConfig.psm1"
Import-Module -Name $UpdateConfig

##########################################################################################
# V A R I A B L E S
##########################################################################################

# names for resource groups
$Biometrics_RG = "Biometrics_RG"

# names for app service plans
$Biometrics_SP = "Biometrics_SP"

# unique names for sites
$BiometricsAPI = $Prefix + "BiometricsAPI" + $Suffix
$BiometricsDashboard = $Prefix + "BioMaxDashboard" + $Suffix

# unique names for cloud services
$AlarmServiceName = $Prefix + "alarmservice" + $Suffix

$SQLDatabase = "BiometricsDb"
$SQLDatabaseTable = "biometrics"
$SQLUserName = "BioMaxUser001"
$SQLPassword = "BioMaxPass001"

# event hub names
$EHInputName = "biometrics"
$EHOutputName = "alarms"

$SQLServerName = $Prefix + "sqlserver" + $Suffix
$ServiceBusNamespace = $Prefix + "sb" + $Suffix
$NotificationHubNamespace = $Prefix + "nh" + $Suffix

##########################################################################################
# F U N C T I O N S
##########################################################################################

Function Select-Subscription()
{
    Param([String] $Subscription)

    Try
    {
        Select-AzureRmSubscription -SubscriptionName $Subscription
    }
    Catch
    {
        Write-Verbose -Message $Error[0].Exception.Message
        Write-Verbose -Message "Exiting due to exception: Subscription Not Selected."
    }
}

##########################################################################################
# M A I N
##########################################################################################

$Error.Clear()

# Mark the start time.
$StartTime = Get-Date

# Select Subscription
Select-Subscription $Subscription

# Create Resource Group
$command = $repo + "\Automation\Common\Create-ResourceGroup.ps1"
&$command $Subscription $Biometrics_RG $AzureLocation

# create app service plans
$command = $repo + "\Automation\Common\Create-AppServicePlan.ps1"
&$command $Subscription $Biometrics_RG $Biometrics_SP $AzureLocation

# Create Cloud Service Container
$command = $repo + "\Automation\Common\Create-CloudService"
&$command $Subscription $AlarmServiceName $Biometrics_RG $AzureLocation

# Create SQL Database
$command = $repo + "\Automation\Common\Create-SQLDatabase.ps1"
&$command $Repo $Subscription $Biometrics_RG $AzureLocation $SQLUserName $SQLPassword $SQLServerName $SQLDatabase

# update the connection string setting in the Biometrics API config file
$path = $repo + "\Microservices\Biometrics\API"
$assembly = "BiometricsAPI"
$ConfigFile = $path + "\$assembly\Web.Config"
$setting = "SQLConnStr"
$SQLConnStr = "Server=tcp:$SQLServerName.database.windows.net,1433;Database=$SQLDatabase;User ID=$SQLUserName@$SQLServerName;Password=$SQLPassword;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;"
Update-Config $repo $ConfigFile $setting $SQLConnStr

# Create Service Bus Namespace, Notification Hub Namespace and Event Hubs and Notification HUb
#$command = $repo + "\Automation\Common\Create-ServiceBus.ps1"
#&$command -Repo $Repo -Subscription $Subscription -SBNamespace $ServiceBusNamespace -NHNamespace $NotificationHubNamespace -AzureLocation $AzureLocation 

# Create Stream Analytics Jobs
$StorageName = $Prefix + "storage" + $Suffix
$command = $repo + "\Automation\Common\Create-StreamAnalytics.ps1"
&$command $Subscription $Biometrics_RG $StorageName $ServiceBusNamespace $EHInputName $EHOutputName $SQLServerName $SQLDatabase $SQLDatabaseTable $SQLUserName $SQLPassword $AzureLocation

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"
