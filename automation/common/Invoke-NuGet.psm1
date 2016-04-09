function Invoke-NuGet { param ($assembly, $projectpath, $repo, $command)

    $nugetLocation = $repo + "\Automation\tools"

    #$source = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
    #$Filename = [System.IO.Path]::GetFileName($source)
    #$nuget = "$nugetLocation\$Filename"
    $nuget = "$nugetLocation\nuget.exe"

    #$wc = New-Object System.Net.WebClient
    #$wc.DownloadFile($source, $dest)

    #if ($command -eq "update")
    #{
    #    $repositoryPath = $repo + "\nugets"
    #    $nugetparams = "update", $projectpath, "-repository", $repositoryPath
    #    & $nuget $nugetparams    
    #}

    if ($command -eq "restoreProjectJson")
    {
        Write-Output "NuGet Restore"
        $projJson = $projectpath + "\" + $assembly + "\" + "project.json"
        $nugetparams = "restore", $projJson, "-SolutionDirectory", $projectpath
        & $nuget $nugetparams
    }

    if ($command -eq "restore")
    {
        Write-Output "NuGet Update"
        #$packagesConfig = $projectpath + "\" + $assembly + "\packages.config"
        $packagesConfig = $projectpath + "\" + $assembly + ".sln"
        $repositoryPath = $repo + "\nugets"
        $nugetparams = "update", $packagesConfig, "-repository", $repositoryPath
        & $nuget $nugetparams  
<#
        Write-Output "NuGet Restore"
        $proj = $projectpath + "\" + $assembly + "\" + $assembly + ".csproj"
        $nugetparams = "restore", $proj, "-SolutionDirectory", $projectpath
        & $nuget $nugetparams
#>
        Write-Output "NuGet Restore"
        $proj = $projectpath + "\"  + $assembly + ".sln"
        $nugetparams = "restore", $proj, "-SolutionDirectory", $projectpath
        & $nuget $nugetparams
    }
    
    if ($command -eq "pack")
    {
        Write-Output "NuGet Spec"
        $proj = $projectpath + "\" + $assembly + "\" + $assembly + ".csproj"
        $nugetparams = "spec", "-f", $proj  
        & $nuget $nugetparams

        Write-Output "NuGet Pack"
        $nugetparams = "pack", $proj  
        & $nuget $nugetparams
    } 
}

Export-ModuleMember -Function Invoke-NuGet