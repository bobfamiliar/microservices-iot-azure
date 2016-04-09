[CmdletBinding()]
param
(
    [parameter(Mandatory = $true)][String]
    $Path,
    [parameter(Mandatory = $true)][String]
    $ResourceGroupName,
    [parameter(Mandatory = $true)][String]
    $AzureLocation,
    [Parameter(Mandatory = $true)][String]
    $DeploymentName,
    [Parameter(Mandatory = $true)][String]
    $SiteName,
    [Parameter(Mandatory = $true)][String]
    $ServicePlan
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
        set-azuresubscription -SubscriptionName $Subscription -CurrentStorageAccountName $DefaultStorage
    }
    Catch
    {
        Write-Verbose -Message $Error[0].Exception.Message
    }
}

##########################################################################################
# M A I N
##########################################################################################

$Error.Clear()

Set-StrictMode -Version 3

$TemplateFile = $Path + "\Automation\templates\d2c2d-arm-template-appservice.json"
$WebDeployPackage = $Path + "\Automation\Deploy\Packages\" + $DeploymentName + "\" + $DeploymentName + ".zip"

# Upload the deploy package to a blob in our storage account, so that
# the MSDeploy extension can access it.  Create the container if it doesn't already exist.
$containerName = 'msdeploypackages'
$blobName = (Get-Date -Format 'ssmmhhddMMyyyy') + '-' + $ResourceGroupName + '-' + $DeploymentName + '-WebDeployPackage.zip'

$storageKey = Get-AzureRmStorageAccountKey -ResourceGroupName $ResourceGroup -AccountName $storageAccountName
$StorageContext = New-AzureStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageKey.Key1

# Use the CurrentStorageAccount which is set by Set-AzureSubscription
if (!(Get-AzureStorageContainer -Context $StorageContext -Name $ContainerName -ErrorAction SilentlyContinue)) 
{
    New-AzureStorageContainer -Context $StorageContext -Name $ContainerName -Permission Off
}              
Set-AzureStorageBlobContent -Context $StorageContext -Blob $blobName -Container $ContainerName -File $WebDeployPackage

# Create a SAS token to add to the blob's URI that we give to MSDeploy. This gives it temporary read-only
#   access to the package.
$webDeployPackageUri = New-AzureStorageBlobSASToken -Context $StorageContext -Container $ContainerName -Blob $blobName -Permission r -FullUri

$ParametersFile = $Path + "\Automation\Templates\d2c2d-arm-template-params-appservice.json"
$JSON = @"
{
    "parameterValues": {
        "azureLocation": "$AzureLocation",
        "appSiteName": "$SiteName",
        "appServicePlan": "$ServicePlan",
        "appServicePlanSku": "Standard",
        "appServicePlanSkuSize": "0",
        "msdeployPackageUri":"$WebDeployPackageUri"
    }
}
"@
$JSON | Set-Content -Path $ParametersFile

# Read the values from the parameters file and create a hashtable. We do this because we need to modify one 
#   of the parameters, otherwise we could pass the file directly to New-AzureResourceGroupDeployment.
$parameters = New-Object -TypeName hashtable
$jsonContent = Get-Content $ParametersFile -Raw | ConvertFrom-Json
$jsonContent.parameterValues | Get-Member -Type NoteProperty | ForEach-Object {
    $parameters.Add($_.Name, $jsonContent.parameterValues.($_.Name))
}

# Create a new resource group (if it doesn't already exist) using our template file and template parameters
New-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName -TemplateFile $TemplateFile -TemplateParameterObject $parameters -Force