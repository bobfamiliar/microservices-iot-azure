param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The name of the Azure Subscription for which you've imported a *.publishingsettings file.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The name of the resource group.")]
    [string]$ResourceGroup,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The name of the Azure Region/Location: East US, Central US, West US.")]
    [string]$AzureLocation,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The common prefix for resources naming: looksfamiliar")]
    [string]$Prefix,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="The suffix which is one of 'dev', 'test' or 'prod'")]
    [string]$Suffix
)

#######################################################################################
# S E T  P A T H
#######################################################################################

$Path = Split-Path -parent $PSCommandPath
$Path = Split-Path -parent $path

##########################################################################################
# F U N C T I O N S
##########################################################################################

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

Function RegisterRP {
    Param(
        [string]$ResourceProviderNamespace
    )

    Write-Host "Registering resource provider '$ResourceProviderNamespace'";
    Register-AzureRmResourceProvider -ProviderNamespace $ResourceProviderNamespace -Force;
}

function GenerateUniqueResourceName()
{
    Param(
        [Parameter(Mandatory=$true,Position=0)] [string] $resourceBaseName
    )
    $name = $resourceBaseName
    $name = "{0}{1:x5}" -f $resourceBaseName, (get-random -max 1048575)
    return $name
}

##########################################################################################
# V A R I A B L E S
##########################################################################################

$storageAccountName = $Prefix + "storage" + $Suffix
$storageAccountType = "Standard_LRS"
$serviceBusNamespace = $Prefix + "sbns" + $Suffix
$serviceBusNamespace = GenerateUniqueResourceName($serviceBusNamespace)
$serviceBusMessageQueue = "messagedrop"
$serviceBusAlarmQueue = "alarms"
$databaseAccount = $Prefix + "docdb" + $Suffix
$iotHubName = $Prefix + "iothub" + $Suffix
#$schema = "$" + "schema"

##########################################################################################
# M A I N
##########################################################################################

$Error.Clear()

# Mark the start time.
$StartTime = Get-Date

$ErrorActionPreference = "Stop"

# sign in
Write-Host "Logging in...";
Login-AzureRmAccount;

# select subscription
Set-AzureRmContext -SubscriptionName $Subscription;

# Register RPs
$resourceProviders = @("microsoft.documentdb","microsoft.storage","microsoft.web");
if($resourceProviders.length) {
    Write-Host "Registering resource providers"
    foreach($resourceProvider in $resourceProviders) {
        RegisterRP($resourceProvider);
    }
}

try
{
    Write-Output "Adding the [Microsoft.ServiceBus.dll] assembly to the script..."
    $scriptPath = Split-Path (Get-Variable MyInvocation -Scope 0).Value.MyCommand.Path
    $packagesFolder = (Split-Path $scriptPath -Parent) + "\automation\packages"
    $assembly = Get-ChildItem $packagesFolder -Include "Microsoft.ServiceBus.dll" -Recurse
    Add-Type -Path $assembly.FullName
    Write-Output "The [Microsoft.ServiceBus.dll] assembly has been successfully added to the script."
}
catch [System.Exception]
{
    Write-Output("Could not add the Microsoft.ServiceBus.dll assembly to the script.")
}

#Create or check for existing resource group
$rg = Get-AzureRmResourceGroup -Name $ResourceGroup -ErrorAction SilentlyContinue 
if(!$rg)
{
    Write-Host "Resource group '$ResourceGroup' does not exist.";
    Write-Host "Creating resource group '$ResourceGroup' in location '$AzureLocation'";
    New-AzureRmResourceGroup -Name $ResourceGroup -Location $AzureLocation
}
else{
    Write-Host "Using existing resource group '$ResourceGroup'";
}

Write-Host "Generating Template Parameter File";

Try
{

        $JSON = @"
{
    "$schema": "http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
        "azureLocation": {
          "value": "$AzureLocation"
        },
        "storageAccountName": {
          "value": "$storageAccountName"
        },
        "storageAccountType": {
          "value": "$storageAccountType",
        },
        "serviceBusNamespace": {
          "value": "$serviceBusNamespace"
        },
        "serviceBusMessageQueue": {
          "value": "$serviceBusMessageQueue"
        },
        "serviceBusAlarmQueue": {
          "value": "$serviceBusAlarmQueue"
        },
        "databaseAccount": {
          "value": "$databaseAccount"
        },
        "iotHubName": {
          "value": "$iotHubName"
        }
    }
}
"@
    $ParamsPath = $Path + "\Automation\Templates\d2c2d-arm-template-params.json"
    $TemplatePath = $Path + "\Automation\Templates\d2c2d-arm-template.json"
    $OutputPath = $Path + "\Automation\provision-$ResourceGroup-output.json"

    $JSON | Set-Content -Path $ParamsPath

    New-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroup -TemplateFile $TemplatePath -TemplateParameterFile $ParamsPath | ConvertTo-Json | Out-File  "$OutputPath"

}
Catch
{
    Write-Verbose -Message $Error[0].Exception.Message
    Write-Verbose -Message "Exiting due to exception: IoTHub Not Created."
}

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime" -Verbose