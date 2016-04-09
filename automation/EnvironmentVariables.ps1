##################################################################
# C O N N E C T I O N  S T R I N G S
##################################################################

$provisionOutputPath = $Path + "\automation\provision-$ResourceGroup-output.json"

$provisionInfo = ConvertFrom-Json -InputObject (Gc $provisionOutputPath -Raw)

$iotHubHostName = $provisionInfo.Outputs.iotHubHostName.Value
$iotHubKey = $provisionInfo.Outputs.iotHubKey.Value
$iotHubConnectionString = $provisionInfo.Outputs.iotHubConnectionString.Value
$docDbURI = $provisionInfo.Outputs.docDbURI.Value
$docDbKey = $provisionInfo.Outputs.docDbKey.Value
$docDbConnectionString = $provisionInfo.Outputs.docDbConnectionString.Value
$storageAccountName = $provisionInfo.Parameters.storageAccountName.Value
$serviceBusNamespace = $provisionInfo.Parameters.serviceBusNamespace.Value
$databaseAccount = $provisionInfo.Parameters.databaseAccount.Value
$iotHubname = $provisionInfo.Parameters.iotHubname.Value
$iothubkeyname = "iothubowner"
$ContainerName = "refdata"
$sbmessagequeue = "messagedrop"
$sbalertsqueue = "alarms"

##################################################################
# A P I  S H A R E D  S E C R E T
##################################################################

$SharedSecret = ""
