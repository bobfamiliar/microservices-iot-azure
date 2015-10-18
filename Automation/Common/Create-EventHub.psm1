function Create-EventHub { param ($RepoPath, $ConnStr, $EventHubName)

	$sbupdate = $RepoPath + "\Automation\Tools\sbupdate\SBUpdate.exe"
	$spupdateparams = "-connstr", $ConnStr, "-eventhub", $EventHubName
	& $sbupdate $spupdateparams
}
Export-ModuleMember -Function Create-EventHub