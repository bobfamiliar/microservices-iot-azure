<#
.Synopsis 
    This PowerShell script builds the common NuGet package Store
.Description 
    This PowerShell script builds the common NuGet package Store
.Notes 
    File Name  : Store.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated
    Requires   : Invoke-MsBuild.psm1
    Requires   : Invoke-NuGetsUpdate.psm1

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Store.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Configuration
    Example:  Debug
.Example
    .\Store.ps1 -repo "c:\users\bob\source\repos\looksfamiliar" -configuration "debug"
.Inputs
    The [Repo] parameter is the path to top level folder of the Git Repo.
    The [Configuration] parameter is the build configuration such as Debug or Release
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The Path to the Git Repo")]
    [string]$repo,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="Build Confiuguration such as Debug or Release")]
    [string]$configuration
)

#######################################################################################
# I M P O R T S
#######################################################################################

$msbuildScriptPath = $repo + "\Automation\Common\Invoke-MsBuild.psm1"
$nugetUpdateScriptPath = $repo + "\Automation\Common\Invoke-UpdateNuGet.psm1"

Import-Module -Name $msbuildScriptPath
Import-Module -Name $nugetUpdateScriptPath

#######################################################################################
# F U C N T I O N S
#######################################################################################

function Build-Status { param ($success, $project)

    $message = ""

    if ($success)
    { 
        $message = $project + " build completed successfully."
    }
    else
    { 
        $message = $project + " build failed. Check the build log file for errors."
    }

    Write-Verbose -Message $message -Verbose
}

Function Copy-Nuget { param ($assembly, $path)

    $nuget = $path + "\" + $assembly + "\bin\" + $configuration + "\*.nupkg"
    Copy-Item $nuget -Destination $packagedrop 
}

Function Build-Project { param ($assembly, $path)

    $sol = $path + "\" + $assembly + ".sln"
    $buildSucceeded = Invoke-MsBuild -Path $sol -MsBuildParameters $msbuildargs
    Build-Status $buildSucceeded $assembly
}

#######################################################################################
# V A R I A B L E S
#######################################################################################

$msbuildargs = "/t:clean /t:Rebuild /p:Configuration=" + $configuration + " /v:normal"
$storepack = $repo + "\packages\store*.*"
$path = $repo + "\Microservices\Common\Store"
$packagedrop = $repo + "\packages"
$assembly = "Store"

#######################################################################################
# C L E A N 
#######################################################################################

Remove-Item $storepack -WhatIf
Remove-Item $storepack -Force

#######################################################################################
# B U I L D
#######################################################################################

Update-NuGet Store Wire $path $repo
Build-Project $assembly $path
Copy-Nuget $assembly $path
