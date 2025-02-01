param(
    [string]$EnvironmentName)

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

function CreateAdGroupIfNotExist {
    param (
        [string]$GroupName,
        [string]$NickName
    )
    $groups = az ad group list --display-name $GroupName | ConvertFrom-Json
    if ($groups.Length -eq 0) {
        $result = az ad group create --display-name $groupName --mail-nickname $NickName | ConvertFrom-Json
        if ($LastExitCode -ne 0) {
            Pop-Location
            throw "Unable to create group $groupName."
        }
        $groupId = $result.id
        Start-Sleep -Seconds 15
    }
    else {
        $groupId = $groups.id
    }
    return $groupId
}

$solutionId = "activitytracker"
Write-Host "get group resource id"
$strId = (GetResource -solutionId $solutionId -environmentName $EnvironmentName -resourceId "app-data").ResourceId
if (!$strId) {
    throw "unable to get storage id $strId"
}
$groupId = CreateAdGroupIfNotExist -GroupName "Activity tracker Users" -NickName "app-activity-tracker-users"
az role assignment create --assignee $groupId --role "Storage Blob Data Contributor" --scope "$strId/blobServices/default/containers/activities"
