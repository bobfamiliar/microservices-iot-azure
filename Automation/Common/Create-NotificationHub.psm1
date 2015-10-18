function Create-NotificationHub { param ($RepoPath, $ConnStr, $NotificationHub)

	$sbupdate = $RepoPath + "\Automation\Tools\sbupdate\SBUpdate.exe"
    $sbupdateparams = "-connstr", $ConnStr, "-notificationhub", $NotificationHub
	& $sbupdate $sbupdateparams
}
Export-ModuleMember -Function Create-NotificationHub