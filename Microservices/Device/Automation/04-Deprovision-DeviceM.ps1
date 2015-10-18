<# 
.Synopsis 
    This PowerShell script de-provisions the DeviceM microservice
.Description 
    This PowerShell script de-provisions the DeviceM microservice
.Notes 
    File Name  : 04-Deprovision-DeviceM.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\04-Deprovision-DeviceM.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/
.Parameter Subscription
    Example:  MySubscription
.Inputs
    The [Subscription] parameter is the name of the client Azure subscription.
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The name of the Azure Subscription.")]
    [string]$Subscription
)

#######################################################################################
# V A R I A B L E S
#######################################################################################

$DeviceM_RG = "DeviceM_RG" 

#######################################################################################
# F U N C T I O N S
#######################################################################################

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

# Select Subscription
Select-Subscription $Subscription

remove-azureresourcegroup -Name $DeviceM_RG -force
    
# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime" -Verbose