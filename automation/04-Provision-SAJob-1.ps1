<#
.Synopsis 
    This PowerShell script provisions a Stream Analytics Job
.Description 
    This PowerShell script provisions a Stream Analytics Job
.Notes 
    File Name  : Provision-SAJob-1.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Provision-SAJob-1.ps1 [Null], [-Full], [-Detailed], [-Examples]

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
             }
          ],
          "transformation":{  
             "name":"$SAJobName",
             "properties":{  
                "streamingUnits":3,
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

$sajobname1 = "d2c2d-messages-queue"
$SAJobQuery1 = "select * into queue from iothub"

$includePath = $Path + "\Automation\EnvironmentVariables.ps1"
."$includePath"

#######################################################################################
# M A I N
#######################################################################################

# Mark the start time.
$StartTime = Get-Date

# get the service bus connection string info
$AzureSBNS = Get-AzureSBNamespace $serviceBusNamespace
$Rule = Get-AzureSBAuthorizationRule -Namespace $serviceBusNamespace 
$SBPolicyName = $Rule.Name
$SBPolicyKey = $Rule.Rule.PrimaryKey

# create the stream analytics job
$SAJobPath = Create-SAJob -SAJobName $sajobname1 -SAJobQuery $SAJobQuery1 -IoTHubName $iothubname -IoTHubKeyName $iothubkeyname -IoTHubKey $iothubkey -AzureLocation $AzureLocation -SBNamespace $serviceBusNamespace -SBQueueName $sbmessagequeue -SBPolicyName $SBPolicyName -SBPolicyKey $SBPolicyKey
New-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroup -Name $sajobname1 -File $SAJobPath -Force
Start-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroup -Name $sajobname1

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime" -Verbose

