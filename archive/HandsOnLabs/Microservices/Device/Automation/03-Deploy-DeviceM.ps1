<#
.Synopsis 
    This PowerShell script deploys the DeviceM Microservice
.Description 
    This PowerShell script deploys the DeviceM Microservice
    You must provide the connection strings from your DocumentDb and Redis Cache instances for this script to function
.Notes 
    File Name  : Deploy-DeviceM.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Deploy-DeviceM.ps1 [Null], [-Full], [-Detailed], [-Examples]

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
    [Parameter(Mandatory=$True, Position=3, HelpMessage="A unique user tag assigned for purposes of the lab")]
    [string]$UserTag,
    [switch]$DeployData
)

#######################################################################################
# V A R I A B L E S
#######################################################################################

$includePath = $Repo + "\HandsOnLabs\Automation\Include-ConnectionStrings.ps1"
."$includePath"

# name for resource group
$HOL_RG = "HOL_RG_" + $UserTag

$DeviceM_SP = $UserTag + "DeviceM_SP" + $UserTag
$DeviceM_DB = "DeviceM"

$DevicePublicAPI = $UserTag + "DevicePublicAPI" + $UserTag
$DeviceAdminAPI = $UserTag + "DeviceAdminAPI" + $UserTag

$Storage_RG = "Storage_RG"
$Storage = $UserTag + "storage" + $UserTag

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
Select-Subscription $Subscription $HOL_RG $Storage

# Load DocumentDb with the device registry
if ($DeployData)
{
    $connStr = $docdbconnstr + "Database=" + $DeviceM_DB
    $command = $Repo + "\Automation\Common\Load-DocDb.ps1"
    &$command -Repo $Repo -Subscription $Subscription -DocDbConnStr $connStr -CollectionName Registry
}

# Package the APIs
.\Package-DeviceM.ps1 $Repo

# Deploy the APIs and update their app settings for documentdb and redis
$command = $Repo + "\Automation\Common\Publish-WebSite.ps1"
&$command -Repo $Repo -ResourceGroupName $HOL_RG -DeploymentName DeviceAdminAPI  -Location $AzureLocation -SiteName $DeviceAdminAPI  -ServicePlan $DeviceM_SP -DocDbURI $docdburi -DocDbKEY $docdbkey -RedisURI $redisuri 
&$command -Repo $Repo -ResourceGroupName $HOL_RG -DeploymentName DevicePublicAPI -Location $AzureLocation -SiteName $DevicePublicAPI -ServicePlan $DeviceM_SP -DocDbURI $docdburi -DocDbKEY $docdbkey -RedisURI $redisuri 

# Mark the finish time.
$FinishTime = Get-Date

# Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"
