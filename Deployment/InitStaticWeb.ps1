param(
    [string]$BUILD_ENV)

$ErrorActionPreference = "Stop"

function GetResource {
    param (
        [string]$solutionId,
        [string]$environmentName,
        [string]$resourceId
    )
        
    $obj = asm lookup resource --asm-rid $resourceId --asm-sol $solutionId --asm-env $environmentName | ConvertFrom-Json
    if ($LastExitCode -ne 0) {        
        throw "Unable to lookup resource."
    }
        
    return $obj
}

dotnet tool install --global AzSolutionManager --version 0.3.0-beta

$solutionId = "activitytracker"

$json = asm lookup resource --asm-rid "app-staticweb" --asm-sol $solutionId --asm-env $BUILD_ENV --logging Info
if ($LastExitCode -ne 0) {
    throw "Error with app-staticweb lookup."
}
$obj = $json | ConvertFrom-Json
$apiKey = az staticwebapp secrets list --name $obj.Name --query "properties.apiKey" | ConvertFrom-Json
"apiKey=$apiKey" >> $env:GITHUB_OUTPUT

$accountName = (GetResource -solutionId $solutionId -environmentName $BUILD_ENV -resourceId "app-data").Name
if (!$accountName) {
    throw "Unable to find app-data resource"
}

$appSettingsContent = [System.Environment]::GetEnvironmentVariable('APPSETTINGS')
$appSettingsContent = $appSettingsContent.Replace("%STORAGENAME%", $accountName)
$appSettingsContent | Out-File -FilePath .\Eklee.ActivityTracker\wwwroot\appsettings.json -Force