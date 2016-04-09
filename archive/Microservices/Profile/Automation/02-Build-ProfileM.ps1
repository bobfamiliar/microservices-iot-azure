<#
.Synopsis 
    This PowerShell script builds the ProfileM Microservice
.Description 
    This PowerShell script builds the ProfileM Microservice
.Notes 
    File Name  : Profile.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated
    Requires   : Invoke-MsBuild.psm1
    Requires   : Invoke-NuGetsUpdate.psm1

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Profile.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Link   
    https://microservices.codeplex.com/

.Parameter Repo
    Example:  c:\users\bob\source\repos\looksfamiliar
.Parameter Configuration
    Example:  Debug
.Example
    .\Profile.ps1 -repo "c:\users\bob\source\repos\looksfamiliar" -configuration "debug"
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

$profilepack = $repo + "\packages\*profile*.*"
Remove-Item $profilepack -WhatIf
Remove-Item $profilepack -Force

#######################################################################################
# M O D E L S
#######################################################################################

$path = $repo + "\Microservices\Profile\Models"
$assembly = "ProfileModels"

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# I N T E R F A C E S
#######################################################################################

$path = $repo + "\Microservices\Profile\Public\Interface"
$assembly = "IProfilePublic"

Update-NuGet IProfilePublic ProfileModels $path $repo
Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Profile\Admin\Interface"
$assembly = "IProfileAdmin"

Update-NuGet IProfileAdmin ProfileModels $path $repo
Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# S E R V I C E S
#######################################################################################

$path = $repo + "\Microservices\Profile\Public\Service"
$assembly = "ProfilePublicService"

Update-NuGet ProfilePublicService Wire $path $repo
Update-NuGet ProfilePublicService Store $path $repo
Update-NuGet ProfilePublicService ProfileModels $path $repo
Update-NuGet ProfilePublicService IProfilePublic $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Profile\Admin\Service"
$assembly = "ProfileAdminService"

Update-NuGet ProfileAdminService Wire $path $repo
Update-NuGet ProfileAdminService Store $path $repo
Update-NuGet ProfileAdminService ProfileModels $path $repo
Update-NuGet ProfileAdminService IProfileAdmin $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# S D K S
#######################################################################################

$path = $repo + "\Microservices\Profile\Public\SDK"
$assembly = "ProfilePublicSDK"

Update-NuGet ProfilePublicSDK Wire $path $repo
Update-NuGet ProfilePublicSDK ProfileModels $path $repo
Update-NuGet ProfilePublicSDK IProfilePublic $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

$path = $repo + "\Microservices\Profile\Admin\SDK"
$assembly = "ProfileAdminSDK"

Update-NuGet ProfileAdminSDK Wire $path $repo
Update-NuGet ProfileAdminSDK ProfileModels $path $repo
Update-NuGet ProfileAdminSDK IProfileAdmin $path $repo

Build-Project $assembly $path
Copy-Nuget $assembly $path

#######################################################################################
# A P I S
#######################################################################################

$path = $repo + "\Microservices\Profile\Public\API"
$assembly = "ProfilePublicAPI"

Update-NuGet ProfilePublicAPI Wire $path $repo
Update-NuGet ProfilePublicAPI Store $path $repo
Update-NuGet ProfilePublicAPI ProfileModels $path $repo
Update-NuGet ProfilePublicAPI IProfilePublic $path $repo
Update-NuGet ProfilePublicAPI ProfilePublicService $path $repo

Build-Project $assembly $path

$path = $repo + "\Microservices\Profile\Admin\API"
$assembly = "ProfileAdminAPI"

Update-NuGet ProfileAdminAPI Wire $path $repo
Update-NuGet ProfileAdminAPI Store $path $repo
Update-NuGet ProfileAdminAPI ProfileModels $path $repo
Update-NuGet ProfileAdminAPI IProfileAdmin $path $repo
Update-NuGet ProfileAdminAPI ProfileAdminService $path $repo

Build-Project $assembly $path
