<#
.Synopsis 
    This PowerShell script provisions API Management
.Description 
    This PowerShell script provisions API Management
.Notes 
    File Name  : Create-APIManagement.ps1
    Author     : Ron Bokleman, Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Create-APIManagement.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Subscription
    Example:  mysubscription
.Parameter ResourceGroupName
    Example:  APIM_RG
.Parameter Orginization
    Example:  Looksfamiliar, Inc.
.Parameter Name
    Example:  looksfamiliar
.Parameter AzureLocation
    Example:  East US
.Example
    .\Create-APIManagement.ps1 -Subscription mysubscription -ResourceGroupName APIM_RG -Orginization "Looksfamiliar, Inc." -Name looksfamiliar -AdminEmail bob@looksfamiliar.com -AzureLocation East US
.Inputs
    The [Subscription] parameter is the name of the Azure subscription.
    The [ResourceGroupName] parameter is the name of the Azure Resource Group
    The [Orginization] parameter is the Orginization whose API's will be under management.
    The [Name] parameter is the name that will be used to define proxy URIs.
    The [AdminEmail] parameter is the Administrators email.
    The [AzureLocation] parameter is the name of the Azure Region/Location: East US, Central US, West US.
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The storage account name.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The resource group name.")]
    [string]$ResourceGroupName,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The Orginization whose API's will be under management.")]
    [string]$Orginization,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The name that will be used to define proxy URIs.")]
    [string]$Name,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="The Administrators email")]
    [string]$AdminEmail,
    [Parameter(Mandatory=$True, Position=5, HelpMessage="The name of the Azure Region/Location: East US, Central US, West US.")]
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

#######################################################################################
# M A I N
#######################################################################################

$Error.Clear()

# Mark the start time.
$StartTime = Get-Date

Select-Subscription $Subscription
Register-AzureProvider -ProviderNamespace Microsoft.ApiManagement -Force
New-AzureApiManagement -ResourceGroupName $ResourceGroupName -Location $AzureLocation -Sku Developer -Organization $Orginization -Name $Name -AdminEmail $AdminEmail

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"