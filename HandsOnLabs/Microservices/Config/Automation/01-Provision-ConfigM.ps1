<# 
.Synopsis 
    This PowerShell script provisions the ConfigM Microservice
.Description 
    This PowerShell script provisions the ConfigM Microservice
.Notes 
    File Name  : Provision-ConfigM.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Provision-ConfigM.ps1 [Null], [-Full], [-Detailed], [-Examples]

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
    [Parameter(Mandatory=$True, Position=3, HelpMessage="A unique user tag assigned for purposes of the lab")]
    [string]$UserTag
)

#######################################################################################
# I M P O R T S
#######################################################################################

$UpdateConfig = $Repo + "\Automation\Common\Invoke-UpdateConfig.psm1"
Import-Module -Name $UpdateConfig

##########################################################################################
# V A R I A B L E S
##########################################################################################

# name for resource group
$HOL_RG = "HOL_RG_" + $UserTag

# names for app service plan
$ConfigM_SP = $UserTag + "ConfigM_SP" + $UserTag

# unique names for sites
$ConfigPublicAPI = $UserTag + "ConfigPublicAPI" + $UserTag
$ConfigAdminAPI = $UserTag + "ConfigAdminAPI" + $UserTag

##########################################################################################
# F U N C T I O N S
##########################################################################################

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

##########################################################################################
# M A I N
##########################################################################################

$Error.Clear()

# mark the start time.
$StartTime = Get-Date

# Select Subscription
Select-Subscription $Subscription

# create app service plans
$command = $Repo + "\Automation\Common\Create-AppServicePlan"
&$command $Subscription $HOL_RG $ConfigM_SP $AzureLocation

# create web site containers
$command = $Repo + "\Automation\Common\Create-WebSite.ps1"
&$command $Subscription $ConfigAdminAPI  $HOL_RG  $ConfigM_SP $AzureLocation
&$command $Subscription $ConfigPublicAPI $HOL_RG  $ConfigM_SP $AzureLocation

#update the BioMaxDashboard config file
$path = $repo + "\Microservices\Biometrics\Dashboard\Biometrics"
$assembly = "BiometricsDashboard"
$ConfigFile = $path + "\$assembly\Web.Config"
$setting = "ConfigM"
$value="https://$ConfigPublicAPI.azurewebsites.net/config"
Update-Config $repo $ConfigFile $setting $value
# mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"
