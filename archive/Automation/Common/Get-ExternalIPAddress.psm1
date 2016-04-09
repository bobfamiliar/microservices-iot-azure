
Function Get-ExternalIPAddress()
{
    $WebClient = New-Object System.Net.WebClient
    Return $WebClient.downloadstring("http://checkip.dyndns.com") -replace "[^\d\.]"
}

Export-ModuleMember -Function Get-ExternalIPAddress