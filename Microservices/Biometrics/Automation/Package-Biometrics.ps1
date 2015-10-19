[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="The Path to the Git Repo")]
    [string]$repo,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="The common prefix for resource naming")]
    [string]$Prefix,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="The suffix for resource naming: 'dev, 'test' or 'prod'")]
    [string]$Suffix
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
# W O R K E R  R O L E
#######################################################################################

$path = $repo + "\Microservices\Biometrics\AlarmsWorker"
$assembly = "BiometricAlarmsWorker"

Publish-CloudService $assembly $path

#######################################################################################
# D A S H B O A R D
#######################################################################################

$path = $repo + "\Microservices\Biometrics\Dashboard\Biometrics"
$assembly = "BiometricsDashboard"

# publish site
Publish-WebSite $assembly $path