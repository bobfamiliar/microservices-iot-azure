<#
.Synopsis 
    This PowerShell script provisions a Cloud Service
.Description 
    This PowerShell script provisions a Cloud Service
.Notes 
    File Name  : Create-CloudService.ps1
    Author     : Ron Bokleman, Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Create-CloudService.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Subscription
    Example:  mysubscription
.Parameter CloudServiceName
    Example:  MyCloudService
.Parameter AzureLocation
    Example:  East US
.Example
    .\Create-AppServicePlan.ps1 -Subscription mysubscription -CloudServiceName MyCloudService -AzureLocation East US
.Inputs
    The [Subscription] parameter is the name of the Azure subscription.
    The [CloudServiceName] parameter is the name of the Cloud Service.
    The [AzureLocation] parameter is the name of the Azure Region/Location: East US, Central US, West US.
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The subscription name.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The Cloud Service name.")]
    [string]$CloudServiceName,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The Resource Group name.")]
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
        Select-AzureSubscription -SubscriptionName $Subscription -ErrorAction Stop

        # List Subscription details if successfully connected.
        Get-AzureSubscription -Current -ErrorAction Stop

        Write-Verbose -Message "Currently selected Azure subscription is: $Subscription."
    }
    Catch
    {
        Write-Verbose -Message $Error[0].Exception.Message
        Write-Verbose -Message "Exiting due to exception: Subscription Not Selected."
    }
}

function Create-CloudService($serviceName, $resourcegroup, $serviceLocation)
{
    Write-Verbose "$(Get-Date –f $timeStampFormat) - Cloud service creation - Started"

    # check for existence
    $service = Get-AzureService -ServiceName $serviceName -ErrorVariable errPrimaryService -ErrorAction "SilentlyContinue"

    if ($service -ne $null)
    {
        Write-Verbose "$(Get-Date –f $timeStampFormat) - Error occurred - $($serviceName) Service already exists "
        return
    }

    # create new service
    New-AzureService -ServiceName $serviceName -Location $serviceLocation -ErrorVariable errPrimaryService -ErrorAction "SilentlyContinue" | Out-Null
    Start-Sleep -Seconds 60

    if ($errPrimaryService[0] -ne $null)
    {
        Write-Verbose "$(Get-Date –f $timeStampFormat) - Error occurred - $($serviceName) Service failed - $errPrimaryService[0] "
        return
    }

    # move the cloud service into the designated resource group
    $r = Get-AzureResource -Name $servicename -OutputObjectFormat New
    Move-AzureResource -ResourceId $r.ResourceId -DestinationResourceGroupName $resourcegroup -Force
    remove-azureresourcegroup -Name $servicename -force

    Write-Verbose "$(Get-Date –f $timeStampFormat) - Cloud service creation - Completed"
} 

#######################################################################################
# M A I N
#######################################################################################

$Error.Clear()

# Mark the start time.
$StartTime = Get-Date

Select-Subscription -Subscription $Subscription

Create-CloudService $CloudServiceName $ResourceGroupName $AzureLocation

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"
