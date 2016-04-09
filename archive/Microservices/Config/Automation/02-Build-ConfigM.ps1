<#
.Synopsis 
    This PowerShell script builds the ConfigM Microservice
.Description 
    This PowerShell script builds the ConfigM Microservice
.Notes 
    File Name  : Config.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated
    Requires   : Invoke-MsBuild.psm1
    Requires   : Invoke-NuGetsUpdate.psm1

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Config.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Configuration
    Example:  Debug
.Example
    .\Config.ps1 -repo "c:\users\bob\source\repos\looksfamiliar" -configuration "debug"
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

$configpack = $repo + "\packages\*config*.*"
Remove-Item $configpack -WhatIf
Remove-Item $configpack -Force

#######################################################################################
# M O D E L S
#######################################################################################

$path = $repo + "\Microservices\Config\Models"
$assembly = "ConfigModels"

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# I N T E R F A C E S
#######################################################################################

$path = $repo + "\Microservices\Config\Public\Interface"
$assembly = "IConfigPublic"

Update-NuGet IConfigPublic ConfigModels $path $repo
Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Config\Admin\Interface"
$assembly = "IConfigAdmin"

Update-NuGet IConfigAdmin ConfigModels $path $repo
Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# S E R V I C E S
#######################################################################################

$path = $repo + "\Microservices\Config\Public\Service"
$assembly = "ConfigPublicService"

Update-NuGet ConfigPublicService Store $path $repo
Update-NuGet ConfigPublicService Wire $path $repo
Update-NuGet ConfigPublicService ConfigModels $path $repo
Update-NuGet ConfigPublicService IConfigPublic $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Config\Admin\Service"
$assembly = "ConfigAdminService"

Update-NuGet ConfigAdminService Wire $path $repo
Update-NuGet ConfigAdminService Store $path $repo
Update-NuGet ConfigAdminService ConfigModels $path $repo
Update-NuGet ConfigAdminService IConfigAdmin $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# S D K S
#######################################################################################

$path = $repo + "\Microservices\Config\Public\SDK"
$assembly = "ConfigPublicSDK"

Update-NuGet ConfigPublicSDK Wire $path $repo
Update-NuGet ConfigPublicSDK ConfigModels $path $repo
Update-NuGet ConfigPublicSDK IConfigPublic $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Config\Admin\SDK"
$assembly = "ConfigAdminSDK"

Update-NuGet ConfigAdminSDK Wire $path $repo
Update-NuGet ConfigAdminSDK ConfigModels $path $repo
Update-NuGet ConfigAdminSDK IConfigAdmin $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# C O N S O L E
#######################################################################################

<#
$path = $repo + "\Microservices\Config\Consoles\ConfigMConsole"
$assembly = "ConfigMConsole"

Update-NuGet ConfigMConsole Wire $path $repo
Update-NuGet ConfigMConsole ConfigModels $path $repo
Update-NuGet ConfigMConsole IConfigAdmin $path $repo
Update-NuGet ConfigMConsole ConfigAdminSDK $path $repo

Build-Project $assembly $path
#>

#######################################################################################
# A P I S
#######################################################################################

$path = $repo + "\Microservices\Config\Public\API"
$assembly = "ConfigPublicAPI"

Update-NuGet ConfigPublicAPI Wire $path $repo
Update-NuGet ConfigPublicAPI Store $path $repo
Update-NuGet ConfigPublicAPI ConfigModels $path $repo
Update-NuGet ConfigPublicAPI IConfigPublic $path $repo
Update-NuGet ConfigPublicAPI ConfigPublicService $path $repo

Build-Project $assembly $path

$path = $repo + "\Microservices\Config\Admin\API"
$assembly = "ConfigAdminAPI"

Update-NuGet ConfigAdminAPI Wire $path $repo
Update-NuGet ConfigAdminAPI Store $path $repo
Update-NuGet ConfigAdminAPI ConfigModels $path $repo
Update-NuGet ConfigAdminAPI IConfigAdmin $path $repo
Update-NuGet ConfigAdminAPI ConfigAdminService $path $repo

Build-Project $assembly $path
