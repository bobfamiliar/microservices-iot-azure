function Create-ServiceBus-Queue { param ($RepoPath, $ConnStr, $QueueName)

	$sbupdate = $RepoPath + "\Automation\Tools\sbupdate\SBUpdate.exe"
    $spupdateparams = "-connstr", $ConnStr, "-queue", $QueueName
	& $sbupdate $spupdateparams
}
Export-ModuleMember -Function Create-ServiceBus-Queue 