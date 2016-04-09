function Update-NuGet { param ($assembly, $packageid, $projectpath, $repo, $netfw)

    $nugetupdate = $repo + "\Automation\tools\NuGetUpdate.exe"

    $proj = $projectpath + "\" + $assembly + "\" + $assembly + ".csproj"
    $sol = $projectpath + "\" + $assembly + ".sln"
    $config = $projectpath + "\" + $assembly + "\packages.config"
    $folder = $projectpath + "\packages"
    $nugets = $repo + "\nugets"

    Write-Output "Running NuGetUpdate"
    Write-output "Package Id : $PackageId"
    Write-output "Project : $proj"
    Write-output "Config : $config"
    Write-output "Folder : $folder"
    Write-output "NuGets : $nugets"
    Write-output "NetFw : $netfw"

	$nugetupdateparams = "-name", $packageid, "-proj", $proj, "-config", $config, "-folder", $folder , "-nugets", $nugets, "-netfw", $netfw, "-verbose"

	& $nugetupdate $nugetupdateparams

    #$nugetparams = "restore", $sol

    #& $nuget $nugetparams
}

Export-ModuleMember -Function Update-NuGet