// Parameters
param stackName string

param storageAccountName string
param keyVaultName string

param storageKeySecretName string
param sasDefinitionName string

param websitePlanId string = 'new'
param websiteName string
param websiteAadClientId string
param websiteConfig array
@secure()
param websiteZipUrl string

param workerConfig array
@allowed([
  'Warning'
  'Information'
])
param workerLogLevel string = 'Warning'
param workerSku string = 'Y1'
@secure()
param workerZipUrl string
@minValue(1)
param workerCount int
param useKeyVaultReference bool

// Variables and output

// Cannot use a Key Vault reference for initial deployment.
// https://github.com/Azure/azure-functions-host/issues/7094
var storageSecretValue = 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${listkeys(storageAccount.id, storageAccount.apiVersion).keys[0].value};EndpointSuffix=core.windows.net'
var storageSecretReference = '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=${storageKeySecretName})'
var workerStorageSecret = useKeyVaultReference ? storageSecretReference : storageSecretValue

output websiteDefaultHostName string = website.properties.defaultHostName
output websiteHostNames array = website.properties.hostNames
output workerDefaultHostNames array = [for i in range(0, workerCount): workers[i].properties.defaultHostName]

var sharedConfig = [
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    value: insights.properties.InstrumentationKey
  }
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: insights.properties.ConnectionString
  }
  {
    name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
    value: '~2'
  }
  {
    name: 'Knapcode.ExplorePackages:HostSubscriptionId'
    value: subscription().subscriptionId
  }
  {
    name: 'Knapcode.ExplorePackages:HostResourceGroupName'
    value: resourceGroup().name
  }
  {
    name: 'Knapcode.ExplorePackages:KeyVaultName'
    value: keyVaultName
  }
  {
    name: 'Knapcode.ExplorePackages:StorageAccountName'
    value: storageAccountName
  }
  {
    name: 'Knapcode.ExplorePackages:StorageConnectionStringSecretName'
    value: storageKeySecretName
  }
  {
    name: 'Knapcode.ExplorePackages:StorageSharedAccessSignatureSecretName'
    value: '${storageAccountName}-${sasDefinitionName}'
  }
  {
    // See: https://github.com/projectkudu/kudu/wiki/Configurable-settings#ensure-update-site-and-update-siteconfig-to-take-effect-synchronously 
    name: 'WEBSITE_ENABLE_SYNC_UPDATE_SITE'
    value: '1'
  }
  {
    name: 'WEBSITE_RUN_FROM_PACKAGE'
    value: '1'
  }
]

// Shared resources
module storageAndKv './storage-and-kv.bicep' = {
  name: '${deployment().name}-storage-and-kv'
  params: {
    storageAccountName: storageAccountName
    keyVaultName: keyVaultName
    identities: [for i in range(0, workerCount + 1): {
      tenantId: i == 0 ? website.identity.tenantId : workers[i - 1].identity.tenantId
      objectId: i == 0 ? website.identity.principalId : workers[i - 1].identity.principalId
    }]
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: storageAccountName
}

resource insights 'Microsoft.Insights/components@2015-05-01' = {
  name: 'ExplorePackages-${stackName}'
  location: resourceGroup().location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

// Website
resource websitePlan 'Microsoft.Web/serverfarms@2020-09-01' = if (websitePlanId == 'new') {
  name: 'ExplorePackages-${stackName}-WebsitePlan'
  location: resourceGroup().location
  sku: {
    name: 'B1'
  }
}

resource website 'Microsoft.Web/sites@2020-09-01' = {
  name: websiteName
  location: resourceGroup().location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: websitePlanId == 'new' ? websitePlan.id : websitePlanId
    clientAffinityEnabled: false
    httpsOnly: true
    siteConfig: {
      webSocketsEnabled: true
      minTlsVersion: '1.2'
      netFrameworkVersion: 'v5.0'
      appSettings: concat([
        {
          name: 'AzureAd:Instance'
          value: 'https://login.microsoftonline.com/'
        }
        {
          name: 'AzureAd:ClientId'
          value: websiteAadClientId
        }
        {
          name: 'AzureAd:TenantId'
          value: 'common'
        }
        {
          // Needed so that the update secrets timer appears enabled in the UI
          name: 'Knapcode.ExplorePackages:HostAppName'
          value: websiteName
        }
      ], sharedConfig, websiteConfig)
    }
  }

  resource deploy 'extensions' = {
    name: any('ZipDeploy') // Workaround per: https://github.com/Azure/bicep/issues/784#issuecomment-817260643
    properties: {
      packageUri: websiteZipUrl
    }
  }
}

// Workers
resource workerPlan 'Microsoft.Web/serverfarms@2020-09-01' = {
  name: 'ExplorePackages-${stackName}-WorkerPlan'
  location: resourceGroup().location
  sku: {
    name: workerSku
  }
}

var workerConfigWithStorage = concat(workerConfig, workerSku == 'Y1' ? [
  {
    name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
    value: workerStorageSecret
  }
] : [])

resource workers 'Microsoft.Web/sites@2020-09-01' = [for i in range(0, workerCount): {
  name: 'ExplorePackages-${stackName}-Worker-${i}'
  location: resourceGroup().location
  kind: 'FunctionApp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: workerPlan.id
    clientAffinityEnabled: false
    httpsOnly: true
    siteConfig: {
      minTlsVersion: '1.2'
      alwaysOn: true
      appSettings: concat([
        {
          name: 'AzureFunctionsJobHost__logging__LogLevel__Default'
          value: workerLogLevel
        }
        {
          name: 'AzureWebJobsFeatureFlags'
          value: 'EnableEnhancedScopes'
        }
        {
          name: 'AzureWebJobsStorage'
          value: workerStorageSecret
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'Knapcode.ExplorePackages:HostAppName'
          value: 'ExplorePackages-${stackName}-Worker-${i}'
        }
        {
          name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
          value: 'false'
        }
      ], sharedConfig, workerConfigWithStorage)
    }
  }
}]

resource perms 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for i in range(0, workerCount): {
  name: guid('FunctionsCanRestartThemselves-${i}')
  scope: resourceGroup()
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'de139f84-1756-47ae-9be6-808fbbe84772')
    principalId: workers[i].identity.principalId
  }
}]

resource workerDeployments 'Microsoft.Web/sites/extensions@2020-09-01' = [for i in range(0, workerCount): {
  name: 'ZipDeploy'
  parent: workers[i]
  properties: {
    packageUri: workerZipUrl
  }
}]
