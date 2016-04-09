function Update-SQLDatabase { param ($RepoPath, $SQLServer, $SQLDatabase, $SQLUSername, $SQLPassword)

	$sqlupdate = $RepoPath + "\Automation\Tools\sqlupdate\SQLUpdate.exe"
	$sqlupdateparams = "-server", $SQLServer, "-database", $SQLDatabase, "-username", $SQLUsername, "-password", $SQLPassword
	& $sqlupdate $sqlupdateparams
}
Export-ModuleMember -Function Update-SQLDatabase 