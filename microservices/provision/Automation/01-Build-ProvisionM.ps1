<#
.Synopsis 
    This PowerShell script builds the ProvisionM Microservice
.Description 
    This PowerShell script builds the ProvisionM Microservice
.Notes 
    File Name  : Build-ProvisionM.ps1
    Author     : Bob Familiar
    Requires   : PowerShell V4 or above, PowerShell / ISE Elevated
    Requires   : Invoke-MsBuild.psm1
    Requires   : Invoke-NuGetsUpdate.psm1

    Please do not forget to ensure you have the proper local PowerShell Execution Policy set:

        Example:  Set-ExecutionPolicy Unrestricted 

    NEED HELP?

    Get-Help .\Build-ProvisionM.ps1 [Null], [-Full], [-Detailed], [-Examples]

.Parameter Path
    Example:  c:\users\bob\source\Paths\looksfamiliar
.Parameter Configuration
    Example:  Debug
.Example
    .\Device.ps1 -Path "c:\users\bob\source\Paths\looksfamiliar" -configuration "debug"
.Inputs
    The [Path] parameter is the path to the top level folder of the Git Path.
    The [Configuration] parameter is the build configuration such as Debug or Release
.Outputs
    Console
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The Path to the Git Path")]
    [string]$path,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="Build configuration such as Debug or Release")]
    [string]$configuration
)

#######################################################################################
# I M P O R T S
#######################################################################################

$msbuildScriptPath = $Path + "\Automation\Common\Invoke-MsBuild.psm1"
$nugetInvokeScriptPath = $Path + "\Automation\Common\Invoke-NuGet.psm1"
$nugetUpdateScriptPath = $Path + "\Automation\Common\Invoke-UpdateNuGet.psm1"

Import-Module -Name $msbuildScriptPath
Import-Module -Name $nugetInvokeScriptPath
Import-Module -Name $nugetUpdateScriptPath

#######################################################################################
# V A R I A B L E S
#######################################################################################

$msbuildargsBuild = "/t:clean /t:Rebuild /p:Configuration=" + $configuration + " /v:normal"
$packagedrop = $Path + "\nugets"

#######################################################################################
# F U N C T I O N S
#######################################################################################

Function Build-Status { param ($success, $project, $operation)

    $message = ""

    if ($success)
    { 
        $message = $project + " " + $operation + " completed successfully."
        Write-Verbose -Message $message -Verbose
    }
    else
    { 
        $message = $project + " " + $operation + " failed. Check the log file for errors."
        Throw $message
    }

}

Function Copy-Nuget { param ($assembly, $path)

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

$provisionpack = $Path + "\nugets\*provision*.*"
Remove-Item $provisionpack -WhatIf
Remove-Item $provisionpack -Force

#######################################################################################
# I N T E R F A C E
#######################################################################################

$assemblypath = $Path + "\Microservices\Provision\Interface"
$assembly = "IProvision"

$packagesFolder = $assemblypath + "\packages\*.*"
Remove-Item $packagesFolder -Recurse -WhatIf -ErrorAction Ignore
Remove-Item $packagesFolder -Recurse -Force -ErrorAction Ignore

Invoke-Nuget $assembly $assemblypath $Path restore
Update-NuGet IProvision MessageModelsNet4 $assemblypath $Path net461
Invoke-Nuget $assembly $assemblypath $Path restore

Build-Project $assembly $assemblypath
Invoke-Nuget $assembly $assemblypath $Path pack
Copy-Nuget $assembly $assemblypath

#######################################################################################
# S E R V I C E
#######################################################################################

$assemblypath = $Path + "\Microservices\Provision\Service"
$assembly = "ProvisionService"

$packagesFolder = $assemblypath + "\packages\*.*"
Remove-Item $packagesFolder -Recurse -WhatIf -ErrorAction Ignore
Remove-Item $packagesFolder -Recurse -Force -ErrorAction Ignore

Invoke-Nuget $assembly $assemblypath $Path restore
Update-NuGet ProvisionService MessageModelsNet4 $assemblypath $Path net461
Update-NuGet ProvisionService Store $assemblypath $Path net461
Update-NuGet ProvisionService IProvision $assemblypath $Path net461
Invoke-Nuget $assembly $assemblypath $Path restore

Build-Project $assembly $assemblypath
Invoke-Nuget $assembly $assemblypath $Path pack
Copy-Nuget $assembly $assemblypath

#######################################################################################
# S D K
#######################################################################################

$assemblypath = $Path + "\Microservices\Provision\SDK"
$assembly = "ProvisionSDK"

$packagesFolder = $assemblypath + "\packages\*.*"
Remove-Item $packagesFolder -Recurse -WhatIf -ErrorAction Ignore
Remove-Item $packagesFolder -Recurse -Force -ErrorAction Ignore

Invoke-Nuget $assembly $assemblypath $Path restore
Update-NuGet ProvisionSDK MessageModelsNet4 $assemblypath $Path net461
Update-NuGet ProvisionSDK Wire $assemblypath $Path net461
Update-NuGet ProvisionSDK IProvision $assemblypath $Path net461
Invoke-Nuget $assembly $assemblypath $Path restore

Build-Project $assembly $assemblypath
Invoke-Nuget $assembly $assemblypath $Path pack
Copy-Nuget $assembly $assemblypath

#######################################################################################
# A P I
#######################################################################################

$assemblypath = $Path + "\Microservices\Provision\API"
$assembly = "ProvisionAPI"

$packagesFolder = $assemblypath + "\packages\*.*"
Remove-Item $packagesFolder -Recurse -WhatIf -ErrorAction Ignore
Remove-Item $packagesFolder -Recurse -Force -ErrorAction Ignore

Invoke-Nuget $assembly $assemblypath $Path restore
Update-NuGet ProvisionAPI MessageModelsNet4 $assemblypath $Path net461
Update-NuGet ProvisionAPI Store $assemblypath $Path net461
Update-NuGet ProvisionAPI IProvision $assemblypath $Path net461
Update-NuGet ProvisionAPI ProvisionService $assemblypath $Path net461
Invoke-Nuget $assembly $assemblypath $Path restore

Build-Project $assembly $assemblypath
