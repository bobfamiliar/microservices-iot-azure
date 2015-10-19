<# 
.Synopsis 
    This PowerShell script provisions the foundational components for the Refernece Implementation
.Description 
    This PowerShell Script provisions the the foundational components for the Refernece Implementation including Storage, DocumentDb, Redis, and API Management along with their associated Resource Groups
.Notes 
    File Name  : Provision.ps1
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

##########################################################################################
# V A R I A B L E S
##########################################################################################

# names for resource groups
$DocDb_RG = "DocDb_RG"
$Redis_RG =  "Redis_RG"
$APIManagement_RG = "APIManagement_RG"

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

#import-module "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\azure.psd1"

# Select Subscription
Select-Subscription $Subscription

# Create Resource Groups
.\..\Common\Create-ResourceGroup.ps1 $Subscription $DocDb_RG $AzureLocation
.\..\Common\Create-ResourceGroup.ps1 $Subscription $Redis_RG $AzureLocation
#.\..\Common\Create-ResourceGroup.ps1 $Subscription $APIManagement_RG $AzureLocation

# Create Storage Account
$StorageName = $Prefix + "storage" + $Suffix
.\..\Common\Create-Storage.ps1 $Subscription $StorageName $AzureLocation

# Create DocumentDb
$DocDbname = $Prefix + "docdb" + $Suffix
.\..\Common\Create-DocumentDb.ps1 $Repo $Subscription $DocDbname $DocDb_RG $AzureLocation

# Create Redis Cache
$RedisCacheName = $Prefix + "redis" + $Suffix
.\..\Common\Create-Redis.ps1 $Subscription $RedisCacheName $Redis_RG $AzureLocation

# Create API Management - NOTE: THIS TAKES A LONG TIME TO PROVISION 
# recommend that you perform this operation manually
#.\..\CommonCreate-APIManagement.ps1 $Subscription $APIManagement_RG $AzureLocation

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime" -Verbose
