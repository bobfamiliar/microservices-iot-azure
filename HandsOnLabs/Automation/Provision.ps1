<# 
.Synopsis 
    This PowerShell script provisions the shared resources for the IoT hands-on lab
.Description 
    This PowerShell script provisions the shared resources for the IoT hands-on lab
.Notes 
    File Name  : Provision-HandsOnLab.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Provision.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Subscription
    Example:  MySubscription
.Parameter AzureLocation
    Example:  East US
.Parameter UserTag
    Example:  user001
.Inputs
    The [Repo] parameter is the path to the Git Repo
    The [Subscription] parameter is the name of the client Azure subscription.
    The [AzureLocation] parameter is the name of the Azure Region/Location to host the Virtual Machines for this subscription.
    The [UserTag] a unique user tag assigned for purposes of the lab
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
    [Parameter(Mandatory=$True, Position=3, HelpMessage="A unique user tag assigned for purposes of the lab")]
    [string]$UserTag
)

##########################################################################################
# V A R I A B L E S
##########################################################################################

# name for resource group
$HOL_RG = "HOL_RG_" + $UserTag

$SQLDatabase = "BiometricsDb"
$SQLDatabaseTable = "biometrics"
$SQLUserName = "BioMaxUser001"
$SQLPassword = "BioMaxPass001"

##########################################################################################
# F U N C T I O N S
##########################################################################################

Function Select-Subscription()
{
    Param([String] $Subscription)

    Try
    {
        Select-AzureSubscription -SubscriptionName $Subscription -ErrorAction Stop -Verbose

        # List Subscription details if successfully connected.
        Get-AzureSubscription -Current -ErrorAction Stop -Verbose

        Write-Verbose -Message "Currently selected Azure subscription is: $Subscription." -Verbose
    }
    Catch
    {
        Write-Verbose -Message $Error[0].Exception.Message -Verbose
        Write-Verbose -Message "Exiting due to exception: Subscription Not Selected." -Verbose
    }
}

##########################################################################################
# M A I N
##########################################################################################

$Error.Clear()

# Mark the start time.
$StartTime = Get-Date

import-module "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\azure.psd1"

# Select Subscription
Select-Subscription $Subscription

# Create Resource Groups
$command = $Repo + "\Automation\Common\Create-ResourceGroup.ps1"
&$command $Subscription $HOL_RG $AzureLocation

# Create Storage Account
$StorageName = $UserTag + "storage" + $UserTag
$command = $Repo + "\Automation\Common\Create-Storage.ps1"
&$command $Subscription $StorageName $AzureLocation

# Create DocumentDb
$DocDbname = $UserTag + "docdb" + $UserTag
$command = $Repo + "\Automation\Common\Create-DocumentDb.ps1"
&$command $Repo $Subscription $DocDbname $HOL_RG $AzureLocation

# Create Redis Cache
$RedisCacheName = $UserTag + "redis" + $UserTag
$command = $Repo + "\Automation\Common\Create-Redis.ps1"
&$command $Subscription $RedisCacheName $HOL_RG $AzureLocation

# Create SQL Database
$SQLServerName = $UserTag + "sqlserver" + $UserTag
$command = $Repo + "\Automation\Common\Create-SQLDatabase.ps1"
&$command $Repo $Subscription $HOL_RG $AzureLocation $SQLUserName $SQLPassword $SQLServerName $SQLDatabase

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime" -Verbose
