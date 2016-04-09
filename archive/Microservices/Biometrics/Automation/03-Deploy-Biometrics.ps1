<#
.Synopsis 
    This PowerShell script deploys the API and UX web sites for the Biomedical Application
.Description 
    This PowerShell script deploys the API and UX web sites for the Biomedical Application
    You must provide the connection strings from your DocumentDb and Redis Cache instances for this script to function
.Notes 
    File Name  : Deploy-Biometrics.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Deploy-Biometrics.ps1 [Null], [-Full], [-Detailed], [-Examples]

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
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The name of the Azure Subscription.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The name of the Azure Region/Location: East US, Central US, West US.")]
    [string]$AzureLocation,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The common prefix for resource naming")]
    [string]$Prefix,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="The suffix for resource naming: 'dev, 'test' or 'prod'")]
    [string]$Suffix
)

#######################################################################################
# I M P O R T S
#######################################################################################

$UpdateConfig = $Repo + "\Automation\Common\Invoke-UpdateConfig.psm1"
Import-Module -Name $UpdateConfig

#######################################################################################
# V A R I A B L E S
#######################################################################################

$storageconnstr = $null
$servicebusconnstr = $null

#include documentdb and redis connections trings
$includePath = $Repo + "\Automation\Include-ConnectionStrings.ps1"
."$includePath"

$Biometrics_RG = "Biometrics_RG"
$Biometrics_SP = "Biometrics_SP"
$BiometricsAPI = $Prefix + "BiometricsAPI" + $Suffix
$BiometricsDashboard = $Prefix + "BioMaxDashboard" + $Suffix

$EHInputName = "biometrics"
$EHName = "alarms"
$NHName = "alarms"

$Storage_RG = "Storage_RG"
$Storage = $Prefix + "storage" + $Suffix

#######################################################################################
# F U N C T I O N S
#######################################################################################

Function Select-Subscription()
{
    Param([String] $Subscription, [String] $ResourceGroupName, [String] $StorageName)

    Try
    {
        Select-AzureRmSubscription -SubscriptionName $Subscription
        Set-AzureRmCurrentStorageAccount -ResourceGroupName $ResourceGroupName -StorageAccountName $StorageName
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
Select-Subscription $Subscription $Storage_RG $Storage

# package the sites and cloud services
.\Package-Biometrics.ps1 $Repo $Prefix $Suffix

# deploy the sites and update their app settings for documentdb and redis
$command = $repo + "\Automation\Common\Publish-WebSite.ps1"
&$command -Repo $Repo -ResourceGroupName $Biometrics_RG -DeploymentName BiometricsAPI -Location $AzureLocation -SiteName $BiometricsAPI -ServicePlan $Biometrics_SP -DocDbURI $docdburi -DocDbKEY $docdbkey -RedisURI $redisuri 
&$command -Repo $Repo -ResourceGroupName $Biometrics_RG -DeploymentName BiometricsDashboard -Location $AzureLocation -SiteName $BiometricsDashboard -ServicePlan $Biometrics_SP -DocDbURI $docdburi -DocDbKEY $docdbkey -RedisURI $redisuri 

# get the connections strings for service bus and notification hub
$SBNamespace = $Prefix + "sb" + $Suffix
$servicebus = Get-AzureSBNamespace -Name $SBNamespace -ErrorAction Stop 
$servicebusconnstr = $servicebus.ConnectionString

#update the BioMax Simulator config file
$path = $repo + "\Microservices\Biometrics\Simulator\BioMaxSimulator"
$assembly = "BioMaxSimulator"
$ConfigFile = $path + "\$assembly\App.Config"
$setting = "Microsoft.ServiceBus.ConnectionString"
Update-Config $repo $ConfigFile $setting $servicebusconnstr
$setting = "EventHub"
Update-Config $repo $ConfigFile $setting $EHInputName
Write-Verbose -Message "BioMax Simulator Config Updated"

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"
