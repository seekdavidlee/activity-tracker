# Introduction

This project demostrates how we leverage Azure RBAC (Role Based Access Control) to read/write to the Azure Storage directly from a Blazor WASM client.

### Cost

By default, we are using the Free tier of static web app (for your personal private use) but we are still paying for usage on Azure Storage, The cost should be minimal if you are only tracking a few activities a day.

### Build Status
![Build status](https://github.com/seekdavidlee/activity-tracker/actions/workflows/app.yml/badge.svg)

## Automated setup

1. Fork this repo
1. Follow the steps listed under [AzSolutionManager (ASM)](#azsolutionmanager-asm).
1. The app need to be configured. Create an single-tenant app registeration with any name. One suggestion is to use `ActivityTracker`.
1. In API permissions, add `Azure Storage` as an API. Select `user_impersonation` and note the default as `Delegated permissions`.
1. Select `Grant admin consent for <Tenant>` to grant admin consent.
1. Under GitHub repo settings, create a new environment named `prod`. Create a config for `APPSETTINGS` with the value listed below. Be sure to update the `<Tenant Id>` and `<Client Id>`. The `%STORAGENAME%` will be replaced with the correct value at deployment time.
1. Start a Github deployment. Once deployment is completed, locate the app registration in Entra and add `Single-page application`. Look for the Azure static web app URL as the URL to add like so `https://<Static web app>/authentication/login-callback`.
1. Perform appropriate role assignments by following the steps in [Post Deployment RBAC](#post-deployment-rbac).
1. Navigate to `https://<Static web app>` with the appropriate user who is assigned the the group.

### APPSETTINGS

```json
{
	"System": {
		"Header": "Client",
		"Footer": "Client 2025"
	},
	"StorageUri": "https://%STORAGENAME%.blob.core.windows.net/",	
	"AzureAd": {
		"Authority": "https://login.microsoftonline.com/<Tenant Id>",
		"ClientId": "<Client Id>",
		"ValidateAuthority": false,
		"LoginMode": "Redirect"
	}
}
```

## AzSolutionManager (ASM)

This project uses AzSolutionManager (ASM) for deployment to Azure Subscription. To use ASM, please follow the steps.

1. Clone Utility and follow the steps in the README to setup ASM.

```
git clone https://github.com/seekdavidlee/az-solution-manager-utils.git
```

2. Load Utility and apply manifest. Pass in dev or prod for variable like so ``` $environmentName = "dev" ```.

```
az login --tenant <TENANT ID>
Push-Location ..\az-solution-manager-utils\; .\LoadASMToSession.ps1; Pop-Location
$a = az account show | ConvertFrom-Json; Invoke-ASMSetup -DIRECTORY Deployment -TENANT $a.tenantId -SUBSCRIPTION $a.Id -ENVIRONMENT $environmentName
Set-ASMGitHubDeploymentToResourceGroup -SOLUTIONID "activitytracker" -ENVIRONMENT $environmentName -TENANT $a.tenantId -SUBSCRIPTION $a.Id
```

## Post Deployment RBAC

The following script will assign RBAC to a group called `Activity tracker Users`.

```powershell
.\ApplyAssignments.ps1 -EnvironmentName <dev or prod>
```

You can now assign users to the right group for access.