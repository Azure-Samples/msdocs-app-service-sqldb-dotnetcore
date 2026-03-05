targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name which is used to generate a short unique hash for each resource')
param name string

@minLength(1)
@description('Primary location for all resources')
param location string

@secure()
@description('SQL Server administrator password')
param databasePassword string = ''

param principalId string = ''

var resourceToken = toLower(uniqueString(subscription().id, name, location))

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: '${name}_group'
  location: location
  tags: { 'azd-env-name': name }
}

module resources 'resources.bicep' = {
  name: 'resources'
  scope: resourceGroup
  params: {
    name: name
    location: location
    resourceToken: resourceToken
    databasePassword: databasePassword
    principalId: principalId
  }
}

output AZURE_LOCATION string = location
output WEB_URI string = resources.outputs.WEB_URI
output CONNECTION_SETTINGS array = resources.outputs.CONNECTION_SETTINGS
output CONTAINER_APP_NAME string = resources.outputs.CONTAINER_APP_NAME
output CONTAINER_APP_ID string = resources.outputs.CONTAINER_APP_ID
output CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.CONTAINER_REGISTRY_ENDPOINT
