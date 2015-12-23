[CmdletBinding()]
param
(
    [parameter(Mandatory = $true)][String]
    $Repo,
    [parameter(Mandatory = $true)][String]
    $ResourceGroupName,
    [parameter(Mandatory = $true)][String]
    $Location,
    [Parameter(Mandatory = $true)][String]
    $DeploymentName,
    [Parameter(Mandatory = $true)][String]
    $SiteName,
    [Parameter(Mandatory = $true)][String]
    $ServicePlan,
    [Parameter(Mandatory = $true)][String]
    $DocDbURI,
    [Parameter(Mandatory = $true)][String]
    $DocDbKEY,
    [Parameter(Mandatory = $true)][String]
    $RedisURI
)

#######################################################################################
# F U N C T I O N S
#######################################################################################

function Set-WebsiteConfiguration
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $ConfigurationFile
    )

    $configJson = Get-Content $ConfigurationFile -Raw | ConvertFrom-Json

    $siteName = $configJson.environmentSettings.webSite.name

    $appSettingsHash=@{}

    $appSettings = $configJson.environmentSettings.webSite.appSettings
    $names = $appSettings | Get-Member -MemberType properties | Select-Object -ExpandProperty name
    $names | foreach `
        -Begin {$appSettingsHash=@{}} `
        -Process { $appSettingsHash.Add($_,$appSettings.$_) } `
        -End {$appSettingsHash}

    Set-AzureWebsite $siteName -AppSettings $appSettingsHash 
}

##########################################################################################
# M A I N
##########################################################################################

$Error.Clear()

Set-StrictMode -Version 3

#Select-Subscription $Subscription

$TemplateFile = $Repo + "\Automation\Deploy\Templates\Deploy-WebSite.json"
$WebDeployPackage = $Repo + "\Automation\Deploy\Packages\" + $DeploymentName + "\" + $DeploymentName + ".zip"

# Upload the deploy package to a blob in our storage account, so that
#   the MSDeploy extension can access it.  Create the container if it
#   doesn't already exist.
$containerName = 'msdeploypackages'
$blobName = (Get-Date -Format 'ssmmhhddMMyyyy') + '-' + $ResourceGroupName + '-' + $DeploymentName + '-WebDeployPackage.zip'

# Use the CurrentStorageAccount which is set by Set-AzureRmSubscription

if (!(Get-AzureStorageContainer $containerName -ErrorAction SilentlyContinue)) 
{
    New-AzureStorageContainer -Name $containerName -Permission Off
}              
Set-AzureStorageBlobContent -Blob $blobName -Container $containerName -File $WebDeployPackage

# Create a SAS token to add to the blob's URI that we give to MSDeploy. This gives it temporary read-only
#   access to the package.
$webDeployPackageUri = New-AzureStorageBlobSASToken -Container $containerName -Blob $blobName -Permission r -FullUri

$ParametersFile = $Repo + "\Automation\Deploy\Templates\" + $SiteName + ".json"
$JSON = @"
{
    "parameterValues": {
        "siteName": "$SiteName",
        "hostingPlanName": "$ServicePlan",
        "siteLocation": "$Location",
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
New-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName -Name $DeploymentName -TemplateFile $TemplateFile -TemplateParameterObject $parameters -Force

# generate the appsettigs config file
$JSON = @"
{
  "environmentSettings": {
    "webSite": {
      "name": "$SiteName",
      "location": "$AzureLocation",
      "appSettings": [
        {
          "docdburi": "$DocDbURI",
          "docdbkey": "$DocDbKEY",
          "redisuri": "$RedisURI"
        }
     ]
    }
  }
}
"@

$AppSettingsFile = $Repo + "\Automation\Deploy\Templates\" + $SiteName + "-appsettings.json"
$JSON | Set-Content -Path $AppSettingsFile

#Update the application settings with values from the JSON file
Set-WebsiteConfiguration $AppSettingsFile
