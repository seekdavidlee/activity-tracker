param(
    [string]$BUILD_ENV)

$ErrorActionPreference = "Stop"

dotnet tool install --global AzSolutionManager --version 0.3.0-beta

function GetResourceAndSetInOutput {
    param ($SolutionId, $ResourceId, $EnvName, $OutputKey, [switch]$UseId, [switch]$ThrowIfMissing)

    $json = asm lookup resource --asm-rid $ResourceId --asm-sol $SolutionId --asm-env $EnvName --logging Info
    if ($LastExitCode -ne 0) {
        throw "Error with resource $ResourceId lookup."
    }

    if (!$json) {

        if ($ThrowIfMissing) {
            throw "Value for $OutputKey is missing!"
        }
        return
    }

    $obj = $json | ConvertFrom-Json

    if ($UseId) {
        $objValue = $obj.ResourceId
    }
    else {
        $objValue = $obj.Name
    }

    if ($ThrowIfMissing -and !$objValue) {
        throw "Value for $OutputKey is missing!"
    }

    "$OutputKey=$objValue" >> $env:GITHUB_OUTPUT

    return
}

$solutionId = "activitytracker"
$json = asm lookup group --asm-sol $solutionId --asm-env $BUILD_ENV --logging Info
if ($LastExitCode -ne 0) {
    throw "Error with group lookup."
}
$obj = $json | ConvertFrom-Json
$groupName = $obj.Name
if (!$groupName) {
    throw "Unable to locate group, json $json"
}

"resourceGroupName=$groupName" >> $env:GITHUB_OUTPUT
"prefix=at00" >> $env:GITHUB_OUTPUT