<#
.Synopsis 
    This PowerShell script builds the D2C2D Message Models
.Description 
    This PowerShell script builds the D2C2D Message Models
.Notes 
    File Name  : Build-Models.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated
    Requires   : Invoke-MsBuild.psm1
    Requires   : Invoke-NuGetsUpdate.psm1

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Build-Models.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Parameter Path
    Example:  c:\users\bob\source\Paths\d2c2d
.Parameter Configuration
    Example:  Debug
.Example
    .\Build-Models.ps1 -Path "c:\users\bob\source\Paths\d2c2d" -configuration "debug"
.Inputs
    The [Path] parameter is the path to the top level folder of the Git Path.
    The [Configuration] parameter is the build configuration such as Debug or Release
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The Path to the Git Path")]
    [string]$Path,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="Build configuration such as Debug or Release")]
    [string]$configuration
)

#######################################################################################
# I M P O R T S
#######################################################################################

$msbuildScriptPath = $Path + "\Automation\Common\Invoke-MsBuild.psm1"
$nugetInvokeScriptPath = $Path + "\Automation\Common\Invoke-NuGet.psm1"

Import-Module -Name $msbuildScriptPath
Import-Module -Name $nugetInvokeScriptPath

#######################################################################################
# V A R I A B L E S
#######################################################################################

$msbuildargsBuild = "/t:clean /t:Rebuild /p:Configuration=" + $configuration + " /v:normal"
$packagedrop = $Path + "\nugets"

#######################################################################################
# F U C N T I O N S
#######################################################################################

Function Build-Status { param ($success, $project, $operation)

    $message = ""

    if ($success)
    { 
        $message = $project + " " + $operation + " completed successfully."
    }
    else
    { 
        $message = $project + " " + $operation + " failed. Check the log file for errors."
    }

    Write-Verbose -Message $message -Verbose
}

Function Copy-Nuget {

    $nuget = ".\*.nupkg"
    Move-Item $nuget -Destination $packagedrop
}

Function Build-Project { param ($assembly, $path)

    $sol = $path + "\" + $assembly + ".sln"
    $buildSucceeded = Invoke-MsBuild -Path $sol -MsBuildParameters $msbuildargsBuild
    Build-Status $buildSucceeded $assembly "build"
}

#######################################################################################
# C L E A N 
#######################################################################################

$devicepack = $Path + "\nugets\*messagemodels*.*"
Remove-Item $devicepack -WhatIf
Remove-Item $devicepack -Force

#######################################################################################
# M O D E L S
#######################################################################################

$assemblypath = $Path + "\models\net4"
$assembly = "messagemodels"

Invoke-Nuget $assembly $assemblypath $Path restorePackages
Build-Project $assembly $assemblypath
Invoke-Nuget $assembly $assemblypath $Path pack
Copy-Nuget $assembly $assemblypath

$assemblypath = $Path + "\models\net5"
$assembly = "messagemodels"

Invoke-Nuget $assembly $assemblypath $Path restoreProjectJson
Build-Project $assembly $assemblypath
Invoke-Nuget $assembly $assemblypath $Path pack
Copy-Nuget $assembly $assemblypath