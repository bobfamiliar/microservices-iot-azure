<#
.Synopsis 
    This PowerShell script deploys the Cloud Services for the Biomedical Application
.Description 
    This PowerShell script deploys the Cloud Services for the Biomedical Application
    You must provide the connection strings from your DocumentDb and Redis Cache instances for this script to function
.Notes 
    File Name  : Deploy-AlarmWorker.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Deploy-AlarmWorker.ps1 [Null], [-Full], [-Detailed], [-Examples]

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

$UpdateCSCFG = $Repo + "\Automation\Common\Invoke-UpdateCSCFG.psm1"
Import-Module -Name $UpdateCSCFG

#######################################################################################
# V A R I A B L E S
#######################################################################################

$storageconnstr = $null
$servicebusconnstr = $null
$notificationhubconnstr = $null

#include documentdb and redis connections trings
$includePath = $Repo + "\Automation\Include-ConnectionStrings.ps1"
."$includePath"

$Biometrics_RG = "Biometrics_RG"
$AlarmServiceName = $Prefix + "alarmservice" + $Suffix

$EHInputName = "biometrics"
$EHName = "alarms"
$NHName = "alarms"

#######################################################################################
# F U N C T I O N S
#######################################################################################

Function Select-Subscription()
{
    Param([String] $Subscription)

    Try
    {
        Select-AzureSubscription -SubscriptionName $Subscription
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

# package the sites and cloud services
.\Package-Biometrics.ps1 $Repo $Prefix $Suffix

# Create a Classic Storage Account for cloud deployments
$StorageName = $Prefix + "csstorage" + $Suffix
$command = $repo + "\Automation\Common\Create-Storage.ps1"
&$command -Subscription $Subscription -StorageAccountName $StorageName -ResourceGroup $Storage_RG -AzureLocation $AzureLocation -Classic:$true

# configure the storage account for cloud deployments
Set-AzureStorageAccount -StorageAccountName $StorageName -Type Standard_GRS -Label $StorageName 
Set-AzureSubscription -SubscriptionName $Subscription -CurrentStorageAccountName $StorageName

# get the connections string for the storage account
$storagekey = (Get-AzureStorageKey –StorageAccountName $storagename).Primary

$storageconnstr = "DefaultEndpointsProtocol=https;AccountName=$storagename;AccountKey=$storagekey"

# get the connections strings for service bus and notification hub
$SBNamespace = $Prefix + "sb" + $Suffix
$NHNamespace = $Prefix + "nh" + $Suffix
$servicebus = Get-AzureSBNamespace -Name $SBNamespace -ErrorAction Stop 
$notificationhub = Get-AzureSBNamespace -Name $NHNamespace -ErrorAction Stop
$servicebusconnstr = $servicebus.ConnectionString
$notificationhubconnstr = $notificationhub.ConnectionString

# format the URL to the ConfigM Microservices
$ConfigPublicAPI = $Prefix + "ConfigPublicAPI" + $Suffix
$configurl = "https://" + $ConfigPublicAPI + ".azurewebsites.net/config"

# path to the configuration file
$BiometricsAlarmConfig = $Repo + "\Automation\Deploy\Packages\BiometricAlarmsWorker\ServiceConfiguration.Cloud.cscfg"

# update the Alarm Worker Role CSCFG file
#Update-CSCFG $repo $BiometricsAlarmConfig "Azure.Storage.ConnectionString" $storageconnstr
Update-CSCFG $repo $BiometricsAlarmConfig "Azure.ServiceBus.ConnectionString" $servicebusconnstr
Update-CSCFG $repo $BiometricsAlarmConfig "NotificationHubConnectionString" $notificationhubconnstr
Update-CSCFG $repo $BiometricsAlarmConfig "EventHubName" $EHName
Update-CSCFG $repo $BiometricsAlarmConfig "NotificationHubName" $NHName
Update-CSCFG $repo $BiometricsAlarmConfig "ConfigM" $configurl
Write-Verbose -Message "Biometrics Alarm Worker Config Updated"

$BiometricsAlarmPackage = $Repo + "\Automation\Deploy\Packages\BiometricAlarmsWorker\AlarmsWorker.cspkg"

# deploy alarm worker cloud service
$deployment = Get-AzureDeployment -ServiceName $AlarmServiceName -Slot Production -ErrorAction silentlycontinue 

if ($deployment.Name -eq $null)
{
    New-AzureDeployment -ServiceName $AlarmServiceName -Package $BiometricsAlarmPackage -Configuration $BiometricsAlarmConfig -Slot Production -Label $AlarmServiceName
}
else
{
    Set-AzureDeployment -Upgrade $AlarmServiceName -Slot Production -Package $BiometricsAlarmPackage  -Configuration $BiometricsAlarmConfig -Label $AlarmServiceName 
}

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"