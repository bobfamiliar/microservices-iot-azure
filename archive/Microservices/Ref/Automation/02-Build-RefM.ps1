<#
.Synopsis 
    This PowerShell script builds the RefM Microservice
.Description 
    This PowerShell script builds the RefM Microservice
.Notes 
    File Name  : Build-Ref.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated
    Requires   : Invoke-MsBuild.psm1
    Requires   : Invoke-NuGetsUpdate.psm1

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Ref.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Configuration
    Example:  Debug
.Example
    .\Build-Ref.ps1 -repo "c:\users\bob\source\repos\looksfamiliar" -configuration "debug"
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

$refpack = $repo + "\packages\*ref*.*"
Remove-Item $refpack -WhatIf
Remove-Item $refpack -Force

#######################################################################################
# M O D E L S
#######################################################################################

$path = $repo + "\Microservices\Ref\Models"
$assembly = "RefModels"

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# I N T E R F A C E S
#######################################################################################

$path = $repo + "\Microservices\Ref\Public\Interface"
$assembly = "IRefPublic"

Update-NuGet IRefPublic RefModels $path $repo
Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Ref\Admin\Interface"
$assembly = "IRefAdmin"

Update-NuGet IRefAdmin RefModels $path $repo
Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# S E R V I C E S
#######################################################################################

$path = $repo + "\Microservices\Ref\Public\Service"
$assembly = "RefPublicService"

Update-NuGet RefPublicService Wire $path $repo
Update-NuGet RefPublicService Store $path $repo
Update-NuGet RefPublicService RefModels $path $repo
Update-NuGet RefPublicService IRefPublic $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Ref\Admin\Service"
$assembly = "RefAdminService"

Update-NuGet RefAdminService Wire $path $repo
Update-NuGet RefAdminService Store $path $repo
Update-NuGet RefAdminService RefModels $path $repo
Update-NuGet RefAdminService IRefAdmin $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# S D K S
#######################################################################################

$path = $repo + "\Microservices\Ref\Public\SDK"
$assembly = "RefPublicSDK"

Update-NuGet RefPublicSDK Wire $path $repo
Update-NuGet RefPublicSDK RefModels $path $repo
Update-NuGet RefPublicSDK IRefPublic $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Ref\Admin\SDK"
$assembly = "RefAdminSDK"

Update-NuGet RefAdminSDK Wire $path $repo
Update-NuGet RefAdminSDK RefModels $path $repo
Update-NuGet RefAdminSDK IRefAdmin $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# A P I S
#######################################################################################

$path = $repo + "\Microservices\Ref\Public\API"
$assembly = "RefPublicAPI"

Update-NuGet RefPublicAPI Wire $path $repo
Update-NuGet RefPublicAPI Store $path $repo
Update-NuGet RefPublicAPI RefModels $path $repo
Update-NuGet RefPublicAPI IRefPublic $path $repo
Update-NuGet RefPublicAPI RefPublicService $path $repo

Build-Project $assembly $path

$path = $repo + "\Microservices\Ref\Admin\API"
$assembly = "RefAdminAPI"

Update-NuGet RefAdminAPI Wire $path $repo
Update-NuGet RefAdminAPI Store $path $repo
Update-NuGet RefAdminAPI RefModels $path $repo
Update-NuGet RefAdminAPI IRefAdmin $path $repo
Update-NuGet RefAdminAPI RefAdminService $path $repo

Build-Project $assembly $path
