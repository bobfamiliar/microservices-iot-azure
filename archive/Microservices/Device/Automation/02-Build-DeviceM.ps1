<#
.Synopsis 
    This PowerShell script builds the DeviceM Microservice
.Description 
    This PowerShell script builds the DeviceM Microservice
.Notes 
    File Name  : Build-DeviceM.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated
    Requires   : Invoke-MsBuild.psm1
    Requires   : Invoke-NuGetsUpdate.psm1

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Build-DeviceM.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Configuration
    Example:  Debug
.Example
    .\Device.ps1 -repo "c:\users\bob\source\repos\looksfamiliar" -configuration "debug"
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

$devicepack = $repo + "\packages\*device*.*"
Remove-Item $devicepack -WhatIf
Remove-Item $devicepack -Force

#######################################################################################
# M O D E L S
#######################################################################################

$path = $repo + "\Microservices\Device\Models"
$assembly = "DeviceModels"

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# I N T E R F A C E S
#######################################################################################

$path = $repo + "\Microservices\Device\Public\Interface"
$assembly = "IDevicePublic"

Update-NuGet IDevicePublic DeviceModels $path $repo
Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Device\Admin\Interface"
$assembly = "IDeviceAdmin"

Update-NuGet IDeviceAdmin DeviceModels $path $repo
Build-Project $assembly $path
Copy-Nuget $assembly $path


#######################################################################################
# S E R V I C E S
#######################################################################################

$path = $repo + "\Microservices\Device\Public\Service"
$assembly = "DevicePublicService"

Update-NuGet DevicePublicService Wire $path $repo
Update-NuGet DevicePublicService Store $path $repo
Update-NuGet DevicePublicService DeviceModels $path $repo
Update-NuGet DevicePublicService IDevicePublic $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Device\Admin\Service"
$assembly = "DeviceAdminService"

Update-NuGet DeviceAdminService Wire $path $repo
Update-NuGet DeviceAdminService Store $path $repo
Update-NuGet DeviceAdminService DeviceModels $path $repo
Update-NuGet DeviceAdminService IDeviceAdmin $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# S D K S
#######################################################################################

$path = $repo + "\Microservices\Device\Public\SDK"
$assembly = "DevicePublicSDK"

Update-NuGet DevicePublicSDK Wire $path $repo
Update-NuGet DevicePublicSDK DeviceModels $path $repo
Update-NuGet DevicePublicSDK IDevicePublic $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Device\Admin\SDK"
$assembly = "DeviceAdminSDK"

Update-NuGet DeviceAdminSDK Wire $path $repo
Update-NuGet DeviceAdminSDK DeviceModels $path $repo
Update-NuGet DeviceAdminSDK IDeviceAdmin $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# A P I S
#######################################################################################

$path = $repo + "\Microservices\Device\Public\API"
$assembly = "DevicePublicAPI"

Update-NuGet DevicePublicAPI Wire $path $repo
Update-NuGet DevicePublicAPI Store $path $repo
Update-NuGet DevicePublicAPI DeviceModels $path $repo
Update-NuGet DevicePublicAPI IDevicePublic $path $repo
Update-NuGet DevicePublicAPI DevicePublicService $path $repo

Build-Project $assembly $path

$path = $repo + "\Microservices\Device\Admin\API"
$assembly = "DeviceAdminAPI"

Update-NuGet DeviceAdminAPI Wire $path $repo
Update-NuGet DeviceAdminAPI Store $path $repo
Update-NuGet DeviceAdminAPI DeviceModels $path $repo
Update-NuGet DeviceAdminAPI IDeviceAdmin $path $repo
Update-NuGet DeviceAdminAPI DeviceAdminService $path $repo

Build-Project $assembly $path
