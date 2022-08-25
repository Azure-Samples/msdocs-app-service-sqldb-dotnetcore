param webAppName string = uniqueString(resourceGroup().id)
param sku string = 'S1'
param linuxFxVersion string = 'DOTNETCORE|6.0'
param location string = resourceGroup().location

var appServicePlanName = toLower('AppServicePlan-${webAppName}')
var webSiteName = toLower('wapp-${webAppName}')

resource myAppServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  properties: {
     reserved: true
  }
  sku: {
    name: sku
  }
  kind: 'linux'
}

resource appService 'Microsoft.Web/sites@2022-03-01' = {
  name: webSiteName
  location: location
  properties: {
    serverFarmId: myAppServicePlan.id
    siteConfig: {
      linuxFxVersion: linuxFxVersion
    }
  }
}
