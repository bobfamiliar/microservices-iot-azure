<#
.Synopsis 
    This PowerShell script builds the common NuGet packages 
.Description 
    This PowerShell script builds the common NuGet packages
.Notes 
    File Name  : Build-Common.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated
    Requires   : Invoke-MsBuild.psm1
    Requires   : Invoke-NuGetsUpdate.psm1

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Build-Common.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Parameter Path
    Example:  c:\users\bob\source\Paths\looksfamiliar
.Parameter Configuration
    Example:  Debug
.Example
    .\Build-Common.ps1 -Path "c:\users\bob\source\Paths\looksfamiliar" -configuration "debug"
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
    [Parameter(Mandatory=$True, Position=1, HelpMessage="Build Confiuguration such as Debug or Release")]
    [string]$configuration
)

#######################################################################################
# I M P O R T S
#######################################################################################

$msbuildScriptPath = $Path + "\Automation\Common\Invoke-MsBuild.psm1"
$nugetUpdateScriptPath = $Path + "\Automation\Common\Invoke-UpdateNuGet.psm1"
$nugetInvokeScriptPath = $Path + "\Automation\Common\Invoke-NuGet.psm1"

Import-Module -Name $msbuildScriptPath
Import-Module -Name $nugetUpdateScriptPath
Import-Module -Name $nugetInvokeScriptPath

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

    Write-Verbose $message -Verbose
}

Function Copy-Nuget {

    $nuget = ".\*.nupkg"
    Move-Item $nuget -Destination $packagedrop
}

Function Build-Project { param ($assembly, $assemblypath)

    $sol = $assemblypath + "\" + $assembly + ".sln"
    $buildSucceeded = Invoke-MsBuild -Path $sol -MsBuildParameters $msbuildargs
    Build-Status $buildSucceeded $assembly
}

#######################################################################################
# C L E A N 
#######################################################################################

$wirepack = $Path + "\nugets\*wire*.*"
Remove-Item $wirepack -WhatIf
Remove-Item $wirepack -Force

$storepack = $Path + "\nugets\*store*.*"
Remove-Item $storepack -WhatIf
Remove-Item $storepack -Force

#######################################################################################
# V A R I A B L E S
#######################################################################################

$msbuildargs = "/t:clean /t:Rebuild /p:Configuration=" + $configuration + " /v:normal"
$packagedrop = $Path + "\nugets"

#######################################################################################
# B U I L D  W I R E
#######################################################################################

$assemblypath = $Path + "\Microservices\Common\Wire"
$assembly = "Wire"

Invoke-Nuget $assembly $assemblypath $Path restore
Build-Project $assembly $assemblypath
Invoke-Nuget $assembly $assemblypath $Path pack
Copy-Nuget $assembly $assemblypath

#######################################################################################
# B U I L D  S T O R E
#######################################################################################

$assemblypath = $Path + "\Microservices\Common\Store"
$assembly = "Store"

Invoke-Nuget $assembly $assemblypath $Path restore
Build-Project $assembly $assemblypath
Invoke-Nuget $assembly $assemblypath $Path pack
Copy-Nuget $assembly $assemblypath
