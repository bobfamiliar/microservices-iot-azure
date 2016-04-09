function Load-DocumentDb { param ($RepoPath, $dataPath, $connStr, $collectionName)

	$dt = $RepoPath + "\Automation\Tools\dt\dt-1.3\dt.exe"
	$dtparams = "/s:JsonFile", "/s.Files:$dataPath\*.json", "/t:DocumentDBBulk", "/t.ConnectionString:$connStr", "/t.Collection:$collectionName", "/t.CollectionTier:S3"
	& $dt $dtparams
}
Export-ModuleMember -Function Load-DocumentDb
