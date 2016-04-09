<#
.Synopsis 
    This PowerShell script provisions an Azure Web Site Container
.Description 
    This PowerShell script provisions an Azure Web Site Container
.Notes 
    File Name  : Create-WebSite.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Create-WebSite.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Subscription
    Example:  mysubscription
.Parameter WebSiteName
    Example:  looksfamiliar
.Parameter ResourceGroupName
    Example:  MyApp_RG
.Parameter ServicePlanName
    Example:  MyApp_SP
.Parameter AzureLocation
    Example:  East US
.Example
    .\Create-WebSite.ps1 -Subscription mysubscription -WebSiteName looksfamiliar -ResourceGroupName MyApp_RG -ServicePlanName MyApp_SP -AzureLocation East US
.Inputs
    The [Subscription] parameter is the name of the Azure subscription.
    The [WebSiteName] parameter is the documentDb account name.
    The [ResourceGroupName] parameter is the name of the Azure Resource Group
    The [ServicePlanName] parameter is the name of the Azure Resource Group
    The [AzureLocation] parameter is the name of the Azure Region/Location: East US, Central US, West US.
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The subscription account name.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The Web Site name.")]
    [string]$WebSiteName,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The resource group name")]
    [string]$ResourceGroupName,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The app service plan name")]
    [string]$ServicePlanName,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="The name of the Azure Region/Location: East US, Central US, West US.")]
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

##########################################################################################
# M A I N
##########################################################################################

$Error.Clear()

# Mark the start time.
$StartTime = Get-Date

# Select Subscription
Select-Subscription $Subscription

New-AzureRmAppServicePlan -Name $ServicePlanName -Location $AzureLocation -ResourceGroupName $ResourceGroupName -Tier Free -NumberofWorkers 1 -WorkerSize Small

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"
