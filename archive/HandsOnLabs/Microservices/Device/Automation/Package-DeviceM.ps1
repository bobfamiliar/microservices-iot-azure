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

#######################################################################################
# V A R I A B L E S
#######################################################################################

$msbuildargsPackage = "/t:Package /P:PackageLocation=" + $repo + "\Automation\Deploy\Packages\"
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

Function Publish-WebSite { param ($assembly, $path)

    $sol = $path + "\" + $assembly + "\" + $assembly + ".csproj"
    $packageArgs = $msbuildargsPackage + $assembly
    $buildSucceeded = Invoke-MsBuild -Path $sol -MsBuildParameters $packageArgs
    Build-Status $buildSucceeded $assembly "package"
}

#######################################################################################
# A P I S
#######################################################################################

$path = $repo + "\Microservices\Device\Public\API"
$assembly = "DevicePublicAPI"

Publish-WebSite $assembly $path

$path = $repo + "\Microservices\Device\Admin\API"
$assembly = "DeviceAdminAPI"

Publish-WebSite $assembly $path