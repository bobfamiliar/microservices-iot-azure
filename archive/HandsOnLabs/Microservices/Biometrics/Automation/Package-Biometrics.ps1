[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The Path to the Git Repo")]
    [string]$repo
)

#######################################################################################
# I M P O R T S
#######################################################################################

$msbuildScriptPath = $repo + "\Automation\Common\Invoke-MsBuild.psm1"
Import-Module -Name $msbuildScriptPath
$UpdateConfig = $Repo + "\Automation\Common\Invoke-UpdateConfig.psm1"
Import-Module -Name $UpdateConfig

#######################################################################################
# V A R I A B L E S
#######################################################################################

$msbuildargsPackage = "/t:Package /P:PackageLocation=" + $repo + "\Automation\Deploy\Packages\"
$msbuildargsPublish = "/t:Publish /p:PublishDir=" + $repo + "\Automation\Deploy\Packages\"

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

Function Publish-WebSite { param ($assembly, $path)

    $sol = $path + "\" + $assembly + "\" + $assembly + ".csproj"
    $packageArgs = $msbuildargsPackage + $assembly
    $buildSucceeded = Invoke-MsBuild -Path $sol -MsBuildParameters $packageArgs
    Build-Status $buildSucceeded $assembly "package"
    Sleep -Seconds 3
}

Function Publish-CloudService { param ($assembly, $path)

    $sol = $path + "\" + $assembly + ".sln"
    $publishArgs = $msbuildargsPublish + $assembly
    $buildSucceeded = Invoke-MsBuild -Path $sol -MsBuildParameters $publishArgs
    Build-Status $buildSucceeded $assembly "package"
    Sleep -Seconds 3
}

#######################################################################################
# A P I S
#######################################################################################

$path = $repo + "\Microservices\Biometrics\API"
$assembly = "BiometricsAPI"
Publish-WebSite $assembly $path

#######################################################################################
# D A S H B O A R D
#######################################################################################

# publish site
$path = $repo + "\Microservices\Biometrics\Dashboard\Biometrics"
$assembly = "BiometricsDashboard"
Publish-WebSite $assembly $path