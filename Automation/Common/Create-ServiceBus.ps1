<#
.Synopsis 
    This PowerShell script provisions a Service Bus Namespace and two Event Hubs, a NotificationHub namespace and a Notificaiton Hub
.Description 
    This PowerShell script provisions a Service Bus Namespace and two Event Hubs, a NotificationHub namespace and a Notificaiton Hub
.Notes 
    File Name  : Create-ServiceBus.ps1
    Author     : Ron Bokleman, Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Create-ServiceBus.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Subscription
    Example:  mysubscription
.Parameter sbNamespace
    Example:  looksfamiliarsb
.Parameter nhnamespace
    Example:  looksfamiliarnh
.Parameter AzureLocation
    Example:  East US
.Example
    .\Create-ServiceBus.ps1 -repo "c:\users\bob\source\repos\looksfamiliar" -Subscription mysubscription -sbnamespace looksfamiliarsb -nhnamespace looksfamiliarnh -AzureLocation East US
.Inputs
    The [Repo] parameter is the path to the top level folder of the Git Repo.
    The [Subscription] parameter is the name of the Azure subscription.
    The [SBNamespace] parameter is the Service Bus Namespace
    The [NHNamespace] parameter is the Notification Hub Namespace
    The [AzureLocation] parameter is the name of the Azure Region/Location: East US, Central US, West US.
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The path to the Git Repo")]
    [string]$Repo,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The name of the Azure Subscription")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The Service Bus Namespace.")]
    [string]$SBNamespace,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The Notification Hub Namespace.")]
    [string]$NHNamespace,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="The name of the Azure Region/Location: East US, Central US, West US.")]
    [string]$AzureLocation

)

#######################################################################################
# I M P O R T S
#######################################################################################

$CreateEventHub = $Repo + "\Automation\Common\Create-EventHub.psm1"
$CreateNotificationHub = $Repo + "\Automation\Common\Create-NotificationHub.psm1"
Import-Module -Name $CreateEventHub
Import-Module -Name $CreateNotificationHub

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

Function Create-ServiceBus-Namespace()
{
    Param  ([String] $Subscription, [String] $AzureLocation, [String] $Namespace)

    Try
    {
        Write-Verbose -Message "[Start] Creating new Azure Service Bus Namepsace: $Namespace in $Subscription."

        $AzureSBNS = New-AzureSBNamespace -Name $Namespace -NamespaceType Messaging -Location $AzureLocation -CreateACSNamespace $True -ErrorAction Stop

        Write-Verbose -Message "[Finish] Created new Azure Service Bus Namepsace: $Namespace, in $AzureLocation."
    }
    Catch # Catching this exception implies that another Azure subscription worldwide, has already claimed this Azure Service Bus Namespace.
    {
        Throw "Namespace, $Namespace in $AzureLocation, is not available! Azure Namespaces must be UNIQUE worldwide. Aborting..."
    } 

    Return $AzureSBNS
}

Function Create-NotificationHub-Namespace()
{
    Param  ([String] $Subscription, [String] $AzureLocation, [String] $Namespace)

    Try
    {
        Write-Verbose -Message "[Start] Creating new Azure Notification Hub Namepsace: $Namespace in $Subscription."

        $AzureNHNS = New-AzureSBNamespace -Name $Namespace -NamespaceType NotificationHub -Location $AzureLocation -CreateACSNamespace $True -ErrorAction Stop

        Write-Verbose -Message "[Finish] Created new Azure Service Bus Namepsace: $Namespace, in $AzureLocation."
    }
    Catch # Catching this exception implies that another Azure subscription worldwide, has already claimed this Azure Service Bus Namespace.
    {
        Throw "Namespace, $Namespace in $AzureLocation, is not available! Azure Namespaces must be UNIQUE worldwide. Aborting..."
    } 

    Return $AzureNHNS
}

#endregion

#######################################################################################
# M A I N
#######################################################################################

$AzureSBNS = $null
$AzureNHNS = $null
$EHContext = $null
$EHBiometrics = "biometrics"
$EHAlarms = "alarms"
$NHAlarms = "alarms"

# Mark the start time.
$StartTime = Get-Date

# Get the executing PowerShell script name for inclusion in Write-Verbose messages.
$PSScriptName = $MyInvocation.MyCommand.Name

# Call Function
Select-Subscription $Subscription

#####################################################################################################
# S E R V I C E  B U S
#####################################################################################################

$AzureSBNS = Create-ServiceBus-Namespace $Subscription $AzureLocation $SBNamespace

$ConnStr = $AzureSBNS.ConnectionString

Create-EventHub -RepoPath $Repo -ConnStr $ConnStr -EventHubName $EHBiometrics
Create-EventHub -RepoPath $Repo -ConnStr $ConnStr -EventHubName $EHAlarms

Write-Verbose -Message "Created Event Hub $EHBiometrics and $EHAlarms in $SBNamespace"


#####################################################################################################
# N O T I F I C A T I O N  H U B
#####################################################################################################

$AzureNHNS = Create-NotificationHub-Namespace $Subscription $AzureLocation $NHNamespace

$ConnStr = $AzureNHNS.ConnectionString

Create-NotificationHub -RepoPath $Repo -ConnStr $ConnStr -NotificationHub $NHAlarms

Write-Verbose -Message "Created Notification Hub $NHAlarms in $NHNamespace"

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"
