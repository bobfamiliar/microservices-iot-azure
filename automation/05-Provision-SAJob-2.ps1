<#
.Synopsis 
    This PowerShell script provisions a Stream Analytics Job
.Description 
    This PowerShell script provisions a Stream Analytics Job
.Notes 
    File Name  : Provision-SAJob-2.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Provision-SAJob-2.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://msdn.microsoft.com/en-us/library/azure/dn835015.aspx
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The storage account name.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The resource group name.")]
    [string]$ResourceGroup,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The Azure Service Bus Name Space.")]
    [string]$AzureLocation,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The prefix for naming standards.")]
    [string]$Prefix,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="The suffix for naming standards.")]
    [string]$Suffix
)

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

Function Create-SAJob()
{
    param (
    [string]$SAJobName,
    [string]$SAJobQuery,
    [string]$iothubname,
    [string]$IoTHubKeyName,
    [string]$IoTHubKey,
    [string]$StorageAccountName,
    [string]$StorageKey,
    [string]$StorageContainer,
    [String]$AzureLocation, 
    [string]$SBNamespace, 
    [string]$SBQueueName, 
    [string]$SBPolicyName, 
    [string]$SBPolicyKey)

    $CreatedDate = Get-Date -Format u

    $JSON = @"
    {  
       "location":"$AzureLocation",
       "properties":{  
          "sku":{  
             "name":"standard"
          },
          "outputStartTime":"$CreatedDate",
          "outputStartMode":"CustomTime",
          "eventsOutOfOrderPolicy":"drop",
          "eventsOutOfOrderMaxDelayInSeconds":10,
          "inputs":[  
             {  
                "name":"iothub",
                "properties":{  
                   "type":"stream",
                   "serialization":{  
                      "type":"JSON",
                      "properties":{  
                         "encoding":"UTF8"
                      }
                   },
                   "datasource":{  
                      "type":"Microsoft.Devices/IotHubs",
                      "properties":{  
                        "consumerGroupName": "$SAJobName",  
                        "iotHubNamespace": "$iothubname",
                        "sharedAccessPolicyKey": "$IotHubKey",
                        "sharedAccessPolicyName": "$IotHubKeyName"
                      }
                   }
                }
             },
             {  
                "name":"refdata",
                "properties":{  
                   "type":"reference",
                   "serialization":{  
                      "type":"JSON",
                      "properties":{  
                         "encoding":"UTF8"
                      }
                   },
                   "datasource":{  
                      "type":"Microsoft.Storage/Blob",
                      "properties": {
                          "storageAccounts": [
                             {
                               "accountName" : "$StorageAccountName",
                               "accountKey" : "$StorageKey"
                             }
                           ],
                           "container":"$StorageContainer",
                           "blobname":"devicerules.json",
                           "pathPattern":"devicerules.json"
                      }
                   }
                }
             }
          ],
          "transformation":{  
             "name":"$SAJobName",
             "properties":{  
                "streamingUnits":6,
                "query": "$SAJobQuery"
             }
          },
        "outputs": [
          {
            "name": "queue",
            "properties": {
              "type": "stream",
              "serialization": {
                "type": "JSON",
                "properties": {
                  "encoding": "UTF8"
                }
              },
              "datasource": {
                "type": "Microsoft.ServiceBus/Queue",
                "properties": {
                    "serviceBusNamespace":"$SBNameSpace",
                    "sharedAccessPolicyName":"$SBPolicyName",
                    "sharedAccessPolicyKey":"$SBPolicyKey",
                    "queueName":"$SBQueueName"
                }
              }
            }
          }
        ]
      }
  }
"@

    $Path = ".\templates\$SAJobName.json"

    $JSON | Set-Content -Path $Path

    Start-Sleep -Seconds 10
    Return $Path
}

#######################################################################################
# S E T  P A T H
#######################################################################################

$Path = Split-Path -parent $PSCommandPath
$Path = Split-Path -parent $path

#######################################################################################
# V A R I A B L E S
#######################################################################################

$SAJobQuery2 = "SELECT
    Stream.Id, 
    Stream.DeviceId, 
    Stream.MessageType, 
    Stream.Longitude, 
    Stream.Latitude, 
    Stream.[Timestamp], 
    Stream.Temperature, 
    Stream.Humidity
INTO
    queue
FROM
    iothub as Stream
JOIN refdata Ref on Stream.MessageType = Ref.MessageType
WHERE ((Stream.Temperature > Ref.TempUpperBound) or
       (Stream.Temperature < Ref.TempLowerBound) or
       (Stream.Humidity > Ref.HumidityUpperBound) or
       (Stream.Humidity < Ref.HumidityLowerBound))"

$sajobname2 = "d2c2d-alarms-queue"

$includePath = $Path + "\Automation\EnvironmentVariables.ps1"
."$includePath"

#######################################################################################
# M A I N
#######################################################################################

# Mark the start time.
$StartTime = Get-Date

$StorageKey = Get-AzureRmStorageAccountKey -ResourceGroupName $ResourceGroup -AccountName $storageAccountName
$StorageContext = New-AzureStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageKey.Key1
New-AzureStorageContainer -Context $StorageContext -Name $ContainerName -Permission Off

# Upload the rules file to blob storage
$refdata = $path + "\automation\deploy\rules\devicerules.json"
Set-AzureStorageBlobContent -Context $StorageContext -Container $ContainerName -File $refdata

# get the service bus connection string information
$AzureSBNS = Get-AzureSBNamespace $serviceBusNamespace
$Rule = Get-AzureSBAuthorizationRule -Namespace $serviceBusNamespace 
$SBPolicyName = $Rule.Name
$SBPolicyKey = $Rule.Rule.PrimaryKey

# create the stream analytics job
$SAJobPath = Create-SAJob -SAJobName $sajobname2 -SAJobQuery $SAJobQuery2 -IoTHubName $iothubName -IoTHubKeyName $iothubkeyname -IoTHubKey $iothubkey -StorageAccountName $storageAccountName -StorageKey $storageKey.Key1 -StorageContainer $ContainerName -AzureLocation $AzureLocation -SBNamespace $serviceBusNamespace -SBQueueName $sbalertsqueue -SBPolicyName $SBPolicyName -SBPolicyKey $SBPolicyKey
New-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroup -Name $sajobname2 -File $SAJobPath -Force
Start-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroup -Name $sajobname2

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime" -Verbose
