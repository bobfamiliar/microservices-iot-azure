<#
.Synopsis 
    This PowerShell script provisions Stream Analytics Jobs
.Description 
    This PowerShell script provisions Stream Analytics Jobs
.Notes 
    File Name  : Create-StreamAnalytics.ps1
    Author     : Ron Bokleman, Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Create-StreamAnalytics.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Subscription
    Example:  mysubscription
.Parameter StorageAccountName
    Example:  mystorage
.Parameter AzureLocation
    Example:  East US
.Example
    .\Create-Storage.ps1 -Subscription mysubscription -StorageAccountName mystorage -AzureLocation East US
.Inputs
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The storage account name.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The resource group name.")]
    [string]$ResourceGroupName,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The Azure Storage Account name for Stream Analytics jobs.")]
    [string]$StorageAccountName,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The Azure Service Bus Name Space.")]
    [string]$AzureSBNameSpace,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="The Input Event Hub Name.")]
    [string]$EHInputName,
    [Parameter(Mandatory=$True, Position=5, HelpMessage="THe Output Event Hub Name")]
    [string]$EHOutputName,
    [Parameter(Mandatory=$True, Position=6, HelpMessage="The SQL Server Name.")]
    [string]$SQLServerName,
    [Parameter(Mandatory=$True, Position=7, HelpMessage="The SQL Database Name.")]
    [string]$SQLDatabaseName,
    [Parameter(Mandatory=$True, Position=8, HelpMessage="The SQL Database Table Name.")]
    [string]$SQLDatabaseTable,
    [Parameter(Mandatory=$True, Position=9, HelpMessage="The SQL Database Username.")]
    [string]$SQLDatabaseUser,
    [Parameter(Mandatory=$True, Position=10, HelpMessage="The SQL Database Password.")]
    [string]$SQLDatabasePassword,
    [Parameter(Mandatory=$True, Position=11, HelpMessage="The name of the Azure Region/Location: East US, Central US, West US.")]
    [string]$AzureLocation
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The storage account name.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The resource group name.")]
    [string]$ResourceGroupName,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The Azure Storage Account name for Stream Analytics jobs.")]
    [string]$StorageAccountName,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The Azure Service Bus Name Space.")]
    [string]$AzureSBNameSpace,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="The Input Event Hub Name.")]
    [string]$EHInputName,
    [Parameter(Mandatory=$True, Position=5, HelpMessage="THe Output Event Hub Name")]
    [string]$EHOutputName,
    [Parameter(Mandatory=$True, Position=6, HelpMessage="The SQL Server Name.")]
    [string]$SQLServerName,
    [Parameter(Mandatory=$True, Position=7, HelpMessage="The SQL Database Name.")]
    [string]$SQLDatabaseName,
    [Parameter(Mandatory=$True, Position=8, HelpMessage="The SQL Database Table Name.")]
    [string]$SQLDatabaseTable,
    [Parameter(Mandatory=$True, Position=9, HelpMessage="The SQL Database Username.")]
    [string]$SQLDatabaseUser,
    [Parameter(Mandatory=$True, Position=10, HelpMessage="The SQL Database Password.")]
    [string]$SQLDatabasePassword,
    [Parameter(Mandatory=$True, Position=11, HelpMessage="The name of the Azure Region/Location: East US, Central US, West US.")]
    [string]$AzureLocation
)

#######################################################################################
# V A R I I B L E S
#######################################################################################

$Storage_RG = "Storage_RG"
$Storage = $Prefix + "storage" + $Suffix

#######################################################################################
# F U N C T I O N S
#######################################################################################

Function Select-Subscription()
{
    Param([String] $Subscription, [String] $ResourceGroupName, [String] $StorageName)

    Try
    {
        Select-AzureRmSubscription -SubscriptionName $Subscription
        Set-AzureRmCurrentStorageAccount -ResourceGroupName $ResourceGroupName -StorageAccountName $StorageName
    }
    Catch
    {
        Write-Verbose -Message $Error[0].Exception.Message
        Write-Verbose -Message "Exiting due to exception: Subscription Not Selected."
    }
}

Function Create-Archive-Messages-JSON()
{

    param ([String]$AzureLocation, [string]$ServiceBusNamespace, [string]$RulePrimaryKey, [string]$EHName, [string]$StorageAccountName, [string]$SAKeyPrimary, [string] $StorageContainer)

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
                "name":"input",
                "properties":{  
                   "type":"stream",
                   "serialization":{  
                      "type":"JSON",
                      "properties":{  
                         "encoding":"UTF8"
                      }
                   },
                   "datasource":{  
                      "type":"Microsoft.ServiceBus/EventHub",
                      "properties":{  
                         "serviceBusNamespace":"$ServiceBusNamespace",
                         "sharedAccessPolicyName":"RootManageSharedAccessKey",
                         "sharedAccessPolicyKey":"$RulePrimaryKey",
                         "eventHubName":"$EHName"
                      }
                   }
                }
             }
          ],
          "transformation":{  
             "name":"biometrics-blob",
             "properties":{  
                "streamingUnits":1,
                "query":
"WITH Device AS (SELECT * FROM input)
SELECT
    Device.deviceid,
    Device.participantid,
    Device.location.longitude,
    Device.location.latitude,
    Device.reading,
    DeviceSensors.ArrayValue.type,
    DeviceSensors.ArrayValue.value
INTO
    output
FROM
    Device
CROSS APPLY GetElements(Device.sensors) AS DeviceSensors"
             }
          },
    "outputs":
      [
        {
          "name": "output",
          "properties": {
            "serialization": {
              "type": "CSV",
              "fieldDelimiter": "Comma",
              "encoding": "UTF8"
            },
            "datasource": {
              "type": "Microsoft.Storage/Blob",
              "storageAccounts": [
                "$StorageAccountName"
              ],
              "properties": {
                "accountName": "$StorageAccountName",
                "accountKey": "$SAKeyPrimary",
                "conatiner": "$StorageContainer",
                "pathPattern": "biometrics-archive"
              }
            }
          }
        }
      ]
  }
}
"@

    $Path = ".\SAJobs\ArchiveMessages.json"

    $JSON | Set-Content -Path $Path

    Start-Sleep -Seconds 10
    Return $Path

}

Function Create-Database-Messages-JSON()
{

    param ([String]$AzureLocation, [string]$ServiceBusNamespace, [string]$RulePrimaryKey, [string]$EHName, [string]$SQLServerName, [string]$SQLDatabaseName, [string] $SQLDatabaseTable, [string] $SQLDatabaseUser, [string] $SQLDatabasePassword)

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
                "name":"input",
                "properties":{  
                   "type":"stream",
                   "serialization":{  
                      "type":"JSON",
                      "properties":{  
                         "encoding":"UTF8"
                      }
                   },
                   "datasource":{  
                      "type":"Microsoft.ServiceBus/EventHub",
                      "properties":{  
                         "serviceBusNamespace":"$ServiceBusNamespace",
                         "sharedAccessPolicyName":"RootManageSharedAccessKey",
                         "sharedAccessPolicyKey":"$RulePrimaryKey",
                         "eventHubName":"$EHName"
                      }
                   }
                }
             }
          ],
          "transformation":{  
             "name":"biometrics-store",
             "properties":{  
                "streamingUnits":1,
                "query":
"WITH Device AS (SELECT * FROM input)
SELECT
    Device.deviceid,
    Device.participantid,
    Device.location.longitude,
    Device.location.latitude,
    Device.reading,
    DeviceSensors.ArrayValue.type,
    DeviceSensors.ArrayValue.value
INTO
    output
FROM
    Device
CROSS APPLY GetElements(Device.sensors) AS DeviceSensors"
             }
          },
          "outputs":[  
           {  
            "name":"output",
            "properties":{  
               "datasource":{  
                  "type":"Microsoft.Sql/Server/Database",
                  "properties":{  
                     "server":"$SQLServerName",
                     "database":"$SQLDatabaseName",
                     "table":"$SQLDatabaseTable",
                     "user":"$SQLDatabaseUser",
                     "password":"$SQLDatabasePassword"
             }
          }
        }
      }
    ]
  }
}
"@

    $Path = ".\SAJobs\DatabaseMessages.json"

    $JSON | Set-Content -Path $Path

    Start-Sleep -Seconds 10
    Return $Path
} 

Function Create-Glucose-Alarm-Messages-JSON()
{

    param ([String]$AzureLocation, [string]$ServiceBusNamespace, [string]$RulePrimaryKey, [string]$EHInputName, [string] $EHOutputName, [string]$StorageAccountName, [string]$SAKeyPrimary)

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
                "name":"input",
                "properties":{  
                   "type":"stream",
                   "serialization":{  
                      "type":"JSON",
                      "properties":{  
                         "encoding":"UTF8"
                      }
                   },
                   "datasource":{  
                      "type":"Microsoft.ServiceBus/EventHub",
                      "properties":{  
                         "serviceBusNamespace":"$AzureSBNameSpace",
                         "sharedAccessPolicyName":"RootManageSharedAccessKey",
                         "sharedAccessPolicyKey":"$RulePrimaryKey",
                         "eventHubName":"$EHInputName"
                      }
                   }
                }
             }
          ],
          "transformation":{  
             "name":"glucose-alarms",
             "properties":{  
                "streamingUnits":1,
                "query":
"WITH Device as (SELECT * from input)
SELECT
    Device.deviceid,
    Device.participantid,
	Device.location.longitude,
    Device.location.latitude,
    Device.reading,
    DeviceSensors.ArrayValue.type,
    DeviceSensors.ArrayValue.value
INTO
    output
FROM
    Device
CROSS APPLY GetElements(Device.sensors) AS DeviceSensors
WHERE
        ((DeviceSensors.ArrayValue.type = 1) AND (DeviceSensors.ArrayValue.value > 180))"
             }
          },
        "outputs": [
          {
            "name": "output",
            "properties": {
              "type": "stream",
              "serialization": {
                "type": "JSON",
                "properties": {
                  "encoding": "UTF8"
                }
              },
              "datasource": {
                "type": "Microsoft.ServiceBus/EventHub",
                "properties": {
                    "serviceBusNamespace":"$AzureSBNameSpace",
                    "sharedAccessPolicyName":"RootManageSharedAccessKey",
                    "sharedAccessPolicyKey":"$RulePrimaryKey",
                    "eventHubName":"$EHOutputName"
                }
              }
            }
          }
        ]
      }
  }
"@

    $Path = ".\SAJobs\GlucoseAlarmMessages.json"

    $JSON | Set-Content -Path $Path

    Start-Sleep -Seconds 10
    Return $Path

} 

Function Create-Heartrate-Alarm-Messages-JSON()
{

    param ([String]$AzureLocation, [string]$ServiceBusNamespace, [string]$RulePrimaryKey, [string]$EHInputName, [string] $EHOutputName, [string]$StorageAccountName, [string]$SAKeyPrimary)

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
                "name":"input",
                "properties":{  
                   "type":"stream",
                   "serialization":{  
                      "type":"JSON",
                      "properties":{  
                         "encoding":"UTF8"
                      }
                   },
                   "datasource":{  
                      "type":"Microsoft.ServiceBus/EventHub",
                      "properties":{  
                         "serviceBusNamespace":"$AzureSBNameSpace",
                         "sharedAccessPolicyName":"RootManageSharedAccessKey",
                         "sharedAccessPolicyKey":"$RulePrimaryKey",
                         "eventHubName":"$EHInputName"
                      }
                   }
                }
             }
          ],
          "transformation":{  
             "name":"heartrate-alarms",
             "properties":{  
                "streamingUnits":1,
                "query":
"WITH Device as (SELECT * from input)
SELECT
    Device.deviceid,
    Device.participantid,
    Device.location.longitude,
    Device.location.latitude,
    Device.reading,
    DeviceSensors.ArrayValue.type,
    DeviceSensors.ArrayValue.value
INTO
    output
FROM
    Device
CROSS APPLY GetElements(Device.sensors) AS DeviceSensors
WHERE
     ((DeviceSensors.ArrayValue.type = 2) AND (DeviceSensors.ArrayValue.value > 170))"
             }
          },
        "outputs": [
          {
            "name": "output",
            "properties": {
              "type": "stream",
              "serialization": {
                "type": "JSON",
                "properties": {
                  "encoding": "UTF8"
                }
              },
              "datasource": {
                "type": "Microsoft.ServiceBus/EventHub",
                "properties": {
                    "serviceBusNamespace":"$AzureSBNameSpace",
                    "sharedAccessPolicyName":"RootManageSharedAccessKey",
                    "sharedAccessPolicyKey":"$RulePrimaryKey",
                    "eventHubName":"$EHOutputName"
                }
              }
            }
          }
        ]
      }
  }
"@

    $Path = ".\SAJobs\HeartrateAlarmMessages.json"

    $JSON | Set-Content -Path $Path

    Start-Sleep -Seconds 10
    Return $Path

} 

Function Create-Temperature-Alarm-Messages-JSON()
{

    param ([String]$AzureLocation, [string]$ServiceBusNamespace, [string]$RulePrimaryKey, [string]$EHInputName, [string] $EHOutputName, [string]$StorageAccountName, [string]$SAKeyPrimary)

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
                "name":"input",
                "properties":{  
                   "type":"stream",
                   "serialization":{  
                      "type":"JSON",
                      "properties":{  
                         "encoding":"UTF8"
                      }
                   },
                   "datasource":{  
                      "type":"Microsoft.ServiceBus/EventHub",
                      "properties":{  
                         "serviceBusNamespace":"$AzureSBNameSpace",
                         "sharedAccessPolicyName":"RootManageSharedAccessKey",
                         "sharedAccessPolicyKey":"$RulePrimaryKey",
                         "eventHubName":"$EHInputName"
                      }
                   }
                }
             }
          ],
          "transformation":{  
             "name":"temperature-alarms",
             "properties":{  
                "streamingUnits":1,
                "query":
"WITH Device as (SELECT * from input)
SELECT
    Device.deviceid,
    Device.participantid,
    Device.location.longitude,
    Device.location.latitude,
    Device.reading,
    DeviceSensors.ArrayValue.type,
    DeviceSensors.ArrayValue.value
INTO
    output
FROM
    Device
CROSS APPLY GetElements(Device.sensors) AS DeviceSensors
WHERE
     ((DeviceSensors.ArrayValue.type = 4) AND (DeviceSensors.ArrayValue.value > 104))"
             }
          },
        "outputs": [
          {
            "name": "output",
            "properties": {
              "type": "stream",
              "serialization": {
                "type": "JSON",
                "properties": {
                  "encoding": "UTF8"
                }
              },
              "datasource": {
                "type": "Microsoft.ServiceBus/EventHub",
                "properties": {
                    "serviceBusNamespace":"$AzureSBNameSpace",
                    "sharedAccessPolicyName":"RootManageSharedAccessKey",
                    "sharedAccessPolicyKey":"$RulePrimaryKey",
                    "eventHubName":"$EHOutputName"
                }
              }
            }
          }
        ]
      }
  }
"@

    $Path = ".\SAJobs\TemperatureAlarmMessages.json"

    $JSON | Set-Content -Path $Path

    Start-Sleep -Seconds 10
    Return $Path

} 

Function Create-Bloodoxygen-Alarm-Messages-JSON()
{

    param ([String]$AzureLocation, [string]$ServiceBusNamespace, [string]$RulePrimaryKey, [string]$EHInputName, [string] $EHOutputName, [string]$StorageAccountName, [string]$SAKeyPrimary)

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
                "name":"input",
                "properties":{  
                   "type":"stream",
                   "serialization":{  
                      "type":"JSON",
                      "properties":{  
                         "encoding":"UTF8"
                      }
                   },
                   "datasource":{  
                      "type":"Microsoft.ServiceBus/EventHub",
                      "properties":{  
                         "serviceBusNamespace":"$AzureSBNameSpace",
                         "sharedAccessPolicyName":"RootManageSharedAccessKey",
                         "sharedAccessPolicyKey":"$RulePrimaryKey",
                         "eventHubName":"$EHInputName"
                      }
                   }
                }
             }
          ],
          "transformation":{  
             "name":"bloodoxygen-alarms",
             "properties":{  
                "streamingUnits":1,
                "query":
"WITH Device as (SELECT * from input)
SELECT
    Device.deviceid,
    Device.participantid,
    Device.location.longitude,
    Device.location.latitude,
    Device.reading,
    DeviceSensors.ArrayValue.type,
    DeviceSensors.ArrayValue.value
INTO
    output
FROM
    Device
CROSS APPLY GetElements(Device.sensors) AS DeviceSensors
WHERE
     ((DeviceSensors.ArrayValue.type = 3) AND (DeviceSensors.ArrayValue.value < 85))"
             }
          },
        "outputs": [
          {
            "name": "output",
            "properties": {
              "type": "stream",
              "serialization": {
                "type": "JSON",
                "properties": {
                  "encoding": "UTF8"
                }
              },
              "datasource": {
                "type": "Microsoft.ServiceBus/EventHub",
                "properties": {
                    "serviceBusNamespace":"$AzureSBNameSpace",
                    "sharedAccessPolicyName":"RootManageSharedAccessKey",
                    "sharedAccessPolicyKey":"$RulePrimaryKey",
                    "eventHubName":"$EHOutputName"
                }
              }
            }
          }
        ]
      }
  }
"@

    $Path = ".\SAJobs\BloodoxygenAlarmMessages.json"

    $JSON | Set-Content -Path $Path

    Start-Sleep -Seconds 10
    Return $Path

} 

#######################################################################################
# M A I N
#######################################################################################

$Error.Clear()

# Mark the start time.
$StartTime = Get-Date

Select-Subscription $Subscription $Storage_RG $Storage

# Get the Azure Service Bus Namespace by that name.
$AzureSBNS = Get-AzureSBNamespace $AzureSBNameSpace

# Get details required for JSON file.
$Rule = Get-AzureSBAuthorizationRule -Namespace $AzureSBNameSpace 
$RulePrimaryKey = $Rule.Rule.PrimaryKey
$StorageAccountKey = Get-AzureStorageKey -StorageAccountName $StorageAccountName
$SAKeyPrimary = $StorageAccountKey.Primary

Write-Verbose -Message "Creating / Updating Stream Analytics Job Biometric Database."
$Path = Create-Database-Messages-JSON $AzureLocation $AzureSBNameSpace $RulePrimaryKey $EHInputName $SQLServerName $SQLDatabaseName $SQLDatabaseTable $SQLDatabaseUser $SQLDatabasePassword
New-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "biometric-store" -File $Path -Force

Write-Verbose -Message "Creating / Updating Stream Analytics Job Glucose Alarm."
$Path = Create-Glucose-Alarm-Messages-JSON $AzureLocation $AzureSBNameSpace $RulePrimaryKey $EHInputName $EHOutputName $StorageAccountName $SAKeyPrimary
New-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "glucose-alarms" -File $Path -Force

Write-Verbose -Message "Creating / Updating Stream Analytics Job Heartrate Alarm."
$Path = Create-Heartrate-Alarm-Messages-JSON $AzureLocation $AzureSBNameSpace $RulePrimaryKey $EHInputName $EHOutputName $StorageAccountName $SAKeyPrimary
New-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "heartrate-alarms" -File $Path -Force

Write-Verbose -Message "Creating / Updating Stream Analytics Job Temperature Alarm."
$Path = Create-Temperature-Alarm-Messages-JSON $AzureLocation $AzureSBNameSpace $RulePrimaryKey $EHInputName $EHOutputName $StorageAccountName $SAKeyPrimary
New-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "temperature-alarms" -File $Path -Force

Write-Verbose -Message "Creating / Updating Stream Analytics Job Blood Oxygen Alarm."
$Path = Create-Bloodoxygen-Alarm-Messages-JSON $AzureLocation $AzureSBNameSpace $RulePrimaryKey $EHInputName $EHOutputName $StorageAccountName $SAKeyPrimary
New-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "bloodoxygen-alarms" -File $Path -Force

$StartDate = Get-Date -Format u

Write-Verbose -Message "Starting Stream Analytics Job Biometric Database."
Start-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "biometric-store" -OutputStartMode CustomTime -OutputStartTime $StartDate
Write-Verbose -Message "Starting Stream Analytics Job Glucose Alarm."
Start-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "glucose-alarms" -OutputStartMode CustomTime -OutputStartTime $StartDate
Write-Verbose -Message "Starting Stream Analytics Job Heartrate Alarm."
Start-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "heartrate-alarms" -OutputStartMode CustomTime -OutputStartTime $StartDate
Write-Verbose -Message "Starting Stream Analytics Job Temperature Alarm."
Start-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "temperature-alarms" -OutputStartMode CustomTime -OutputStartTime $StartDate
Write-Verbose -Message "Starting Stream Analytics Job Blood Oxygen Alarm."
Start-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "bloodoxygen-alarms" -OutputStartMode CustomTime -OutputStartTime $StartDate

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"
