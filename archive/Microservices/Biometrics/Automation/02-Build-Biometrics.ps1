<#
.Synopsis 
    This PowerShell script builds the Biometrics API and Web Application
.Description 
    This PowerShell script builds the BiometricsM API and Web Application
.Notes 
    File Name  : Build-Biometrics.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated
    Requires   : Invoke-MsBuild.psm1
    Requires   : Invoke-NuGetsUpdate.psm1

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Build-Biometrics.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Configuration
    Example:  Debug
.Example
    .\Biometrics.ps1 -repo "c:\users\bob\source\repos\looksfamiliar" -configuration "debug"
.Inputs
    The [Repo] parameter is the path to the top level folder of the Git Repo.
    The [Configuration] parameter is the build configuration such as Debug or Release
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The Path to the Git Repo")]
    [string]$repo,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="Build configuration such as Debug or Release")]
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
# V A R I A B L E S
#######################################################################################

$msbuildargsBuild = "/t:clean /t:Rebuild /p:Configuration=" + $configuration + " /v:normal"
$packagedrop = $repo + "\packages"

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

Function Copy-Nuget { param ($assembly, $path)

    $nuget = $path + "\" + $assembly + "\bin\" + $configuration + "\*.nupkg"
    Copy-Item $nuget -Destination $packagedrop
}

Function Build-Project { param ($assembly, $path)

    $sol = $path + "\" + $assembly + ".sln"
    $buildSucceeded = Invoke-MsBuild -Path $sol -MsBuildParameters $msbuildargsBuild
    Build-Status $buildSucceeded $assembly "build"
}

#######################################################################################
# C L E A N 
#######################################################################################

$biopack = $repo + "\packages\*biometrics*.*"
Remove-Item $biopack -WhatIf
Remove-Item $biopack -Force

#######################################################################################
# M O D E L S
#######################################################################################

$path = $repo + "\Microservices\Biometrics\Models"
$assembly = "BiometricsModels"

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# I N T E R F A C E S
#######################################################################################

$path = $repo + "\Microservices\Biometrics\Interface"
$assembly = "IBiometrics"

Update-NuGet IBiometrics BiometricsModels $path $repo
Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# S E R V I C E S
#######################################################################################

$path = $repo + "\Microservices\Biometrics\Service"
$assembly = "BiometricsService"

Update-NuGet BiometricsService BiometricsModels $path $repo
Update-NuGet BiometricsService IBiometrics $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# A P I S
#######################################################################################

$path = $repo + "\Microservices\Biometrics\API"
$assembly = "BiometricsAPI"

Update-NuGet BiometricsAPI BiometricsModels $path $repo
Update-NuGet BiometricsAPI IBiometrics $path $repo
Update-NuGet BiometricsAPI BiometricsService $path $repo

Build-Project $assembly $path

#######################################################################################
# S I M U L A T O R
#######################################################################################

$path = $repo + "\Microservices\Biometrics\Simulator\BioMaxSimulator"
$assembly = "BioMaxSimulator"

Update-NuGet BioMaxSimulator Wire $path $repo
Update-NuGet BioMaxSimulator ConfigModels $path $repo
Update-NuGet BioMaxSimulator IConfigPublic $path $repo
Update-NuGet BioMaxSimulator ConfigPublicSDK $path $repo
Update-NuGet BioMaxSimulator ProfileModels $path $repo
Update-NuGet BioMaxSimulator IProfilePublic $path $repo
Update-NuGet BioMaxSimulator ProfilePublicSDK $path $repo
Update-NuGet BioMaxSimulator DeviceModels $path $repo
Update-NuGet BioMaxSimulator IDeviceAdmin $path $repo
Update-NuGet BioMaxSimulator DeviceAdminSDK $path $repo

Build-Project $assembly $path

#######################################################################################
# W O R K E R  R O L E
#######################################################################################

$path = $repo + "\Microservices\Biometrics\AlarmsWorker"
$assembly = "BiometricAlarmsWorker"

Update-NuGet BiometricAlarmsWorker Wire $path $repo
Update-NuGet BiometricAlarmsWorker BiometricsModels $path $repo
Update-NuGet BiometricAlarmsWorker ConfigModels $path $repo
Update-NuGet BiometricAlarmsWorker IConfigPublic $path $repo
Update-NuGet BiometricAlarmsWorker ConfigPublicSDK $path $repo
Update-NuGet BiometricAlarmsWorker ProfileModels $path $repo
Update-NuGet BiometricAlarmsWorker IProfilePublic $path $repo
Update-NuGet BiometricAlarmsWorker ProfilePublicSDK $path $repo

Build-Project $assembly $path

#######################################################################################
# D A S H B O A R D
#######################################################################################

$path = $repo + "\Microservices\Biometrics\Dashboard\Biometrics"
$assembly = "BiometricsDashboard"

Update-NuGet BiometricsDashboard BiometricsModels $path $repo
Update-NuGet BiometricsDashboard Wire $path $repo
Update-NuGet BiometricsDashboard ConfigModels $path $repo
Update-NuGet BiometricsDashboard IConfigPublic $path $repo
Update-NuGet BiometricsDashboard ConfigPublicSDK $path $repo

Build-Project $assembly $path