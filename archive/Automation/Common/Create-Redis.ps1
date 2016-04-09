<#
.Synopsis 
    This PowerShell script provisions an instance of Azure Redis Cache
.Description 
    This PowerShell script provisions an instance of Azure Redis Cache
.Notes 
    File Name  : Create-Redis.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Create-Redis.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Subscription
    Example:  mysubscription
.Parameter RedisCacheName
    Example:  looksfamiliarcache
.Parameter ResourceGroupName
    Example:  Redis_RG
.Parameter AzureLocation
    Example:  East US
.Example
    .\Create-Redis.ps1 -Subscription mysubscription -RedisCacheName looksfamiliarcache -ResourceGroupName Redis_RG -AzureLocation East US
.Inputs
    The [Subscription] parameter is the name of the Azure subscription.
    The [RedisCacheName] parameter is the documentDb account name.
    The [ResourceGroupName] parameter is the name of the Azure Resource Group
    The [AzureLocation] parameter is the name of the Azure Region/Location: East US, Central US, West US.
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The storage account name.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The redis cache account name.")]
    [string]$RedisCacheName,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The resource group name")]
    [string]$ResourceGroupName,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The name of the Azure Region/Location: East US, Central US, West US.")]
    [string]$AzureLocation
)

#######################################################################################
# F U N C T I O N S
#######################################################################################

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

# Create Redis Cache Account
New-AzureRmRedisCache -Location $AzureLocation -Name $RedisCacheName -ResourceGroupName $ResourceGroupName -Size 250MB -Sku Basic

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"

