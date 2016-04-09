<#
.Synopsis 
    This PowerShell script provisions an instance of SQL Database
.Description 
    This PowerShell script provisions an instance of SQL Database
.Notes 
    File Name  : Create-SQLDatabase.ps1
    Author     : Ron Bokleman, Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Create-SQLDatabase.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Subscription
    Example:  mysubscription
.Parameter ResourceGroupName
    Example:  DocDb_RG
.Parameter AzureLocation
    Example:  East US
.Parameter SQLUserName
    Example:  bob
.Parameter SQLPassword
    Example:  p@ssw0rd!
.Parameter SQLServerName
    Example:  MySQLServer
.Parameter SQLDatabaseName
    Example:  MyDatabase
.Example
    .\Create-SQLDatabase.ps1 -repo "c:\users\bob\source\repos\looksfamiliar" -Subscription mysubscription -ResourceGroupName DocDb_RG -AzureLocation East US -SQLUser bob -SQLPassword p@ssw0rd! -SQLServerName $MySQLServer -SQLDatabaseName $MyDatabase
.Inputs
    The [Repo] parameter is the path to the top level folder of the Git Repo.
    The [Subscription] parameter is the name of the Azure subscription.
    The [ResourceGroupName] parameter is the name of the Azure Resource Group
    The [AzureLocation] parameter is the name of the Azure Region/Location: East US, Central US, West US.
    The [SQLUserName] parameter is the SQL Admin Account 
    The [SQLPassword] parameter is the password for the SQL Admin Account 
    The [SQLServerName] parameter is the SQL Server Name
    The [SQLDatabaseName] parameter is the Database Name 
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The path to the Git Repo.")]
    [string]$Repo,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The name of the Azure Subscription.")]
    [string]$Subscription,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The name of the Resource Group.")]
    [string]$ResourceGroupName,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="The name of the Azure Region/Location: East US, Central US, West US.")]
    [ValidatePattern({^[a-zA-Z0-9]})]
    [string]$AzureLocation,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="The SQL Admin Account.")]
    [string]$SQLUserName,
    [Parameter(Mandatory=$True, Position=5, HelpMessage="The password for the SQL Admin Account.")]
    [ValidatePattern({^[a-zA-Z0-9]})]
    [string]$SQLPassword,
    [Parameter(Mandatory=$True, Position=6, HelpMessage="The SQL Server Name.")]
    [string]$SQLServerName,
    [Parameter(Mandatory=$True, Position=7, HelpMessage="The SQL Database Name.")]
    [string]$SQLDatabaseName
)

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

Function Get-ExternalIPAddress()
{
    $WebClient = New-Object System.Net.WebClient
    Return $WebClient.downloadstring("http://checkip.dyndns.com") -replace "[^\d\.]"
}

#######################################################################################
# I M P O R T S
#######################################################################################

$UpdateSQLDatabase = $Repo + "\Automation\Common\Update-SQLDatabase.psm1"
Import-Module -Name $UpdateSQLDatabase

#######################################################################################
# M A I N
#######################################################################################

$Error.Clear()

# Mark the start time.
$StartTime = Get-Date

# Call Function
Select-Subscription $Subscription

# create the credentials object
$SQLSecurePassword = ConvertTo-SecureString $SQLPassword -AsPlainText -Force
$Credentials = New-Object System.Management.Automation.PSCredential($SQLUserName, $SQLSecurePassword)

# Create an Azure SQL Database Server Instance
New-AzureRmSqlServer -ServerName $SQLServerName -SqlAdministratorCredentials $Credentials -ResourceGroupName $ResourceGroupName -Location $AzureLocation
Start-Sleep -Seconds 60

#Allow Azure Services (this is set to No by default)
New-AzureRmSqlServerFirewallRule -ResourceGroupName $ResourceGroupName -ServerName $SQLServerName -AllowAllAzureIPs
Start-Sleep -Seconds 60

#create the Azure SQL Database
New-AzureRmSqlDatabase -ResourceGroupName $ResourceGroupName -DatabaseName $SQLDatabaseName -Edition Standard -ServerName $SQLServerName -MaxSizeBytes 268435456000
Start-Sleep -Seconds 60

$TcpIPAddress = Get-ExternalIPAddress

# Create SQL Database Server Firewall Rule for this client computer ONLY.
New-AzureRmSqlServerFirewallRule -ResourceGroupName $ResourceGroupName -ServerName $SQLServerName -FirewallRuleName $env:COMPUTERNAME -StartIpAddress $TcpIPAddress -EndIpAddress $TcpIPAddress
Start-Sleep -Seconds 60

# create the schema in the database
Update-SQLDatabase -Repo $Repo -SQLServer $SQLServerName -SQLDatabase $SQLDatabaseName -SQLUsername $SQLUserName -SQLPassword $SQLPassword

# Mark the finish time.
$FinishTime = Get-Date

#Console output
$TotalTime = ($FinishTime - $StartTime).TotalSeconds
Write-Verbose -Message "Elapse Time (Seconds): $TotalTime"