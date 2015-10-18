<#
.Synopsis 
    This PowerShell script provisions an instance of Azure DocumentDb
.Description 
    This PowerShell script provisions an instance of Azure DocumentDb
.Notes 
    File Name  : Create-DocumentDb.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Create-DocumentDb.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Subscription
    Example:  mysubscription
.Parameter DocDbName
    Example:  looksfamiliardb
.Parameter ResourceGroupName
    Example:  DocDb_RG
.Parameter AzureLocation
    Example:  East US
.Example
    .\Create-DocumentDb.ps1 -repo "c:\users\bob\source\repos\looksfamiliar" -Subscription mysubscription -DocDbName looksfamiliardb -ResourceGroupName DocDb_RG -AzureLocation East US
.Inputs
    The [Repo] parameter is the path to the top level folder of the Git Repo.
    The [Subscription] parameter is the name of the Azure subscription.
    The [DocDbName] parameter is the documentDb account name.
    The [ResourceGroupName] parameter is the name of the Azure Resource Group
    The [AzureLocation] parameter is the name of the Azure Region/Location: East US, Central US, West US.
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The path to the Git Repo.")]
    [string]$Repo,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The subsription name.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The documentDb account name.")]
    [string]$DocDbName,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The resource group name")]
    [string]$ResourceGroupName,
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

Function Create-DocumentDb
{
    Param([String] $Repo, [String] $Name, [String] $Group, [String] $Location)

    Try
    {

        $JSON = @"
{
        "$schema": "http://schema.management.azure.com/schemas/2014-04-01-preview/deploymentParameters.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
        "databaseAccountName": {
            "value": "$Name"
        },
        "location": {
            "value": "$Location"
        }
    }
}
"@
        $ParamsPath = $Repo + "\Automation\Provision\Templates\DocDb.param.json"
        $TemplatePath = $Repo + "\Automation\Provision\Templates\DocDb.json"

        $JSON | Set-Content -Path $ParamsPath

        Register-AzureProvider -ProviderNamespace Microsoft.DocumentDb -Force

        New-AzureResourceGroupDeployment -ResourceGroupName $Group -TemplateFile $TemplatePath -TemplateParameterFile $ParamsPath
    }
    Catch
    {
        Write-Verbose -Message $Error[0].Exception.Message
        Write-Verbose -Message "Exiting due to exception: DocumentDb Not Created."
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

# Create DocumentDb
Create-DocumentDb -Repo $Repo -Name $DocDbName -Group $ResourceGroupName -Location $AzureLocation

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"
