<#
.Synopsis 
    This PowerShell script provisions an Azure Resource Group
.Description 
    This PowerShell script provisions an Azure Resource Group
.Notes 
    File Name  : Create-ResourceGroup.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Create-ResourceGroup.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Subscription
    Example:  mysubscription
.Parameter ResourceGroupName
    Example:  MyRg_RG
.Parameter AzureLocation
    Example:  East US
.Example
    .\Create-ResourceGroup.ps1 -Subscription mysubscription -ResourceGroupName MyRg_RG -AzureLocation East US
.Inputs
    The [Subscription] parameter is the name of the Azure subscription.
    The [ResourceGroupName] parameter is the name of the Azure Resource Group
    The [AzureLocation] parameter is the name of the Azure Region/Location: East US, Central US, West US.
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The subscription name.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The resource group name.")]
    [string]$ResourceGroupName,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The name of the Azure Region, e.g. East US, Central US, West US.")]
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
        Select-AzureRmSubscription -SubscriptionName $Subscription -ErrorAction Stop
    }
    Catch
    {
        Write-Verbose -Message $Error[0].Exception.Message
        Write-Verbose -Message "Exiting due to exception: Subscription Not Selected."
    }
}


Function Create-ResourceGroup
{
    Param([String] $Name, [String] $Location)

    Try
    {
        New-AzureRmResourceGroup -Name $Name -Location $Location -Force -ErrorAction Stop
    }
    Catch
    {
        Write-Verbose -Message $Error[0].Exception.Message
        Write-Verbose -Message "Exiting due to exception: Resource Group Not Created."
    }

}

#######################################################################################
# M A I N
#######################################################################################

$Error.Clear()

# Mark the start time.
$StartTime = Get-Date

Select-Subscription $Subscription

Create-ResourceGroup $ResourceGroupName $AzureLocation

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"