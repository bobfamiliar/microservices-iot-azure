<# 
.Synopsis 
    This PowerShell script provisions the API Management 
.Description 
    This PowerShell script provisions the API Management
.Notes 
    File Name  : 05-Provision-APIManagement.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\05-Provision-APIManagemen.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Parameter Subscription
    Example:  MySubscription
.Parameter AzureLocation
    Example:  East US
.Parameter Prefix
    Example:  looksfamiliar
.Parameter Suffix
    Example:  test
.Paramater APIAdminEmail
    Example:  bobf@bluemetal.com
.Inputs
    The [Subscription] parameter is the name of the client Azure subscription.
    The [ResourceGroup] parameter is the name of the Azure Resource group to deploy into
    The [AzureLocation] parameter is the name of the Azure Region/Location to host the Virtual Machines for this subscription.
    The [Prefix] parameter is the common prefix that will be used to name resources
    The [Suffix] parameter is one of 'dev', 'test' or 'prod'
    The [APIAdminEmail] paramater is the email of the API Adminstrator
.Outputs
    Console
#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The name of the subscription")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The name of the Resource Group.")]
    [string]$ResourceGroup,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The name of the Azure Region/Location: East US, Central US, West US.")]
    [string]$AzureLocation,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="The common prefix for resources naming: looksfamiliar")]
    [string]$Prefix,
    [Parameter(Mandatory=$True, Position=5, HelpMessage="The suffix which is one of 'dev', 'test' or 'prod'")]
    [string]$Suffix,
    [Parameter(Mandatory=$True, Position=6, HelpMessage="The email of the API Adminstrator")]
    [string]$APIAdminEmail
)

#######################################################################################
# F U N C T I O N S
#######################################################################################

Function Select-Subscription()
{
    Param([String] $Subscription)

    Try
    {
        Set-AzureRmContext  -SubscriptionName $Subscription -ErrorAction Stop
    }
    Catch
    {
        Write-Verbose -Message $Error[0].Exception.Message
    }
}

#######################################################################################
# S E T  P A T H
#######################################################################################

$Path = Split-Path -parent $PSCommandPath
$Path = Split-Path -parent $path

#######################################################################################
# M A I N 
#######################################################################################

$Error.Clear()

# Mark the start time.
$StartTime = Get-Date

$Orginization = "d2c2d"
$APIName = $Prefix + "d2c2d" + $Suffix

Select-Subscription $Subscription

# Create API Management 
New-AzureRmAPIManagement -ResourceGroupName $ResourceGroup -Location $AzureLocation -Sku Developer -Organization $Orginization -Name $APIName -AdminEmail $APIAdminEmail

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime" -Verbose