function Update-CSCFG { param ($repo, $file, $setting, $value)

	$cscfgupdate = $repo + "\Automation\Tools\CSCFGUpdate.exe"

	$cscfgupdateparams = "-file", $file, "-setting", $setting, "-value", $value

	& $cscfgupdate $cscfgupdateparams
}

Export-ModuleMember -Function Update-CSCFG