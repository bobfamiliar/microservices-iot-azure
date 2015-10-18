function Update-Config { param ($repo, $file, $setting, $value)

	$configupdate = $repo + "\Automation\Tools\ConfigUpdate.exe"

	$configupdateparams = "-file", $file, "-setting", $setting, "-value", $value

	& $configupdate $configupdateparams
}

Export-ModuleMember -Function Update-Config