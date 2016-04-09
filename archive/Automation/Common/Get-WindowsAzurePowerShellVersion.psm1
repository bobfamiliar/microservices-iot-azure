function Get-WindowsAzurePowerShellVersion
{
[CmdletBinding()]
Param ()
 
## - Section to query local system for Windows Azure PowerShell version already installed:
Write-Host "`r`nWindows Azure PowerShell Installed version: " -ForegroundColor 'Yellow';
(Get-Module -ListAvailable | Where-Object{ $_.Name -eq 'Azure' }) `
| Select Version, Name, Author | Format-List;
 
## - Section to query web Platform installer for the latest available Windows Azure PowerShell version:
Write-Host "Windows Azure PowerShell available download version: " -ForegroundColor 'Green';
[reflection.assembly]::LoadWithPartialName("Microsoft.Web.PlatformInstaller") | Out-Null;
$ProductManager = New-Object Microsoft.Web.PlatformInstaller.ProductManager;
$ProductManager.Load(); $ProductManager.Products `
| Where-object{
($_.Title -match "Windows Azure PowerShell") `
-and ($_.Author -eq 'Microsoft Corporation')
} `
| Select-Object Version, Title, Published, Author | Format-List;
};
Export-ModuleMember -Function Get-WindowsAzurePowerShellVersion