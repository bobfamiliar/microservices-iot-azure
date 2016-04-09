function Update-NuGet { param ($assembly, $packageid, $projectpath, $repo)

	$nugetupdate = $repo + "\Automation\tools\NuGetUpdate.exe"
    $nuget = $repo + "\Automation\tools\nuget.exe"

    $proj = $projectpath + "\" + $assembly + "\" + $assembly + ".csproj"
    $sol = $projectpath + "\" + $assembly + ".sln"
    $config = $projectpath + "\" + $assembly + "\packages.config"
    $folder = $projectpath + "\packages"
    $nugets = $repo + "\packages"

	$nugetupdateparams = "-name", $packageid, "-proj", $proj, "-config", $config, "-folder", $folder , "-nugets", $nugets

	& $nugetupdate $nugetupdateparams

    $nugetparams = "restore", $sol

    & $nuget $nugetparams
}

Export-ModuleMember -Function Update-NuGet