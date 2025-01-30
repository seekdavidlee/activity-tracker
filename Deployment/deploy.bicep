param prefix string
param strName string = ''
param appName string = ''
param location string = resourceGroup().location
param customDomainName string = ''

var strNameStr = empty(strName) ? '${prefix}${uniqueString(resourceGroup().name)}' : strName
var appNameStr = empty(appName) ? '${prefix}${uniqueString(resourceGroup().name)}' : appName

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: strNameStr
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
  }
}

resource storageContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  name: 'activities'
  parent: storageAccountBlobServices
  properties: {
    publicAccess: 'None'
  }
}
var defaultHostname = empty(customDomainName)
  ? 'https://${staticwebapp.properties.defaultHostname}'
  : 'https://${customDomainName}'

resource storageAccountBlobServices 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    cors: {
      corsRules: [
        {
          allowedOrigins: [defaultHostname]
          allowedMethods: [
            'POST'
            'GET'
            'OPTIONS'
            'HEAD'
            'PUT'
            'MERGE'
            'DELETE'
          ]
          maxAgeInSeconds: 120
          exposedHeaders: [
            '*'
          ]
          allowedHeaders: [
            '*'
          ]
        }
      ]
    }
  }
}

resource staticwebapp 'Microsoft.Web/staticSites@2024-04-01' = {
  name: appNameStr
  location: location
  sku: {
    tier: 'Free'
    name: 'Free'
  }
  properties: {}
}

resource customDomain 'Microsoft.Web/staticSites/customDomains@2024-04-01' = if (!empty(customDomainName)) {
  name: customDomainName
  parent: staticwebapp
}
