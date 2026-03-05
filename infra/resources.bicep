param name string
param location string
param resourceToken string
param principalId string
@secure()
param databasePassword string

var appName = '${name}-${resourceToken}'
var containerAppName = toLower(appName)
var containerRegistryName = toLower('acr${resourceToken}001')
var sqlConnectionString = 'Server=tcp:${dbserver.name}.database.windows.net,1433;Initial Catalog=${dbserver::db.name};Persist Security Info=False;User ID=${dbserver.properties.administratorLogin};Password=${databasePassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;'

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2024-01-01' = {
  location: location
  name: '${appName}Vnet'
  properties: {
    addressSpace: {
      addressPrefixes: ['10.0.0.0/16']
    }
    subnets: [
      {
        name: 'cache-subnet'
        properties: {
          addressPrefix: '10.0.0.0/24'
          privateEndpointNetworkPolicies: 'Disabled'
        }
      }
      {
        name: 'aca-subnet'
        properties: {
          addressPrefix: '10.0.1.0/24'
          delegations: [
            {
              name: 'dlg-containerapps'
              properties: {
                serviceName: 'Microsoft.App/environments'
              }
            }
          ]
        }
      }
      {
        name: 'aca-infra-subnet'
        properties: {
          addressPrefix: '10.0.4.0/23'
          delegations: [
            {
              name: 'dlg-containerapps-infra'
              properties: {
                serviceName: 'Microsoft.App/environments'
              }
            }
          ]
        }
      }
      {
        name: 'database-subnet'
        properties: {
          addressPrefix: '10.0.2.0/24'
          privateEndpointNetworkPolicies: 'Disabled'
        }
      }
      {
        name: 'vault-subnet'
        properties: {
          addressPrefix: '10.0.3.0/24'
          privateEndpointNetworkPolicies: 'Disabled'
        }
      }
    ]
  }
  resource subnetForDb 'subnets' existing = {
    name: 'database-subnet'
  }
  resource subnetForVault 'subnets' existing = {
    name: 'vault-subnet'
  }
  resource subnetForAca 'subnets' existing = {
    name: 'aca-infra-subnet'
  }
  resource subnetForCache 'subnets' existing = {
    name: 'cache-subnet'
  }
}

resource privateDnsZoneKeyVault 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: 'privatelink.vaultcore.azure.net'
  location: 'global'
  resource vnetLink 'virtualNetworkLinks@2020-06-01' = {
    location: 'global'
    name: '${appName}-vaultlink'
    properties: {
      virtualNetwork: {
        id: virtualNetwork.id
      }
      registrationEnabled: false
    }
  }
}

resource privateDnsZoneDB 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: 'privatelink.database.windows.net'
  location: 'global'
  dependsOn: [
    virtualNetwork
  ]
  resource privateDnsZoneLinkDB 'virtualNetworkLinks@2020-06-01' = {
    name: '${appName}-dblink'
    location: 'global'
    properties: {
      virtualNetwork: {
        id: virtualNetwork.id
      }
      registrationEnabled: false
    }
  }
}

resource privateDnsZoneCache 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: 'privatelink.redis.cache.windows.net'
  location: 'global'
  dependsOn: [
    virtualNetwork
  ]
  resource privateDnsZoneLinkCache 'virtualNetworkLinks@2020-06-01' = {
    name: '${appName}-cachelink'
    location: 'global'
    properties: {
      virtualNetwork: {
        id: virtualNetwork.id
      }
      registrationEnabled: false
    }
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: '${take(replace(appName, '-', ''), 17)}-vault'
  location: location
  properties: {
    enableRbacAuthorization: true
    tenantId: subscription().tenantId
    sku: { family: 'A', name: 'standard' }
    publicNetworkAccess: 'Disabled'
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'None'
    }
  }
}

resource vaultPrivateEndpoint 'Microsoft.Network/privateEndpoints@2023-04-01' = {
  name: '${appName}-vault-privateEndpoint'
  location: location
  properties: {
    subnet: {
      id: virtualNetwork::subnetForVault.id
    }
    privateLinkServiceConnections: [
      {
        name: '${appName}-vault-privateEndpoint'
        properties: {
          privateLinkServiceId: keyVault.id
          groupIds: ['vault']
        }
      }
    ]
  }
  resource privateDnsZoneGroup 'privateDnsZoneGroups@2024-01-01' = {
    name: 'default'
    properties: {
      privateDnsZoneConfigs: [
        {
          name: 'vault-config'
          properties: {
            privateDnsZoneId: privateDnsZoneKeyVault.id
          }
        }
      ]
    }
  }
}

resource keyVaultSecretUserRoleRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: '4633458b-17de-408a-b874-0445c86b69e6'
}

resource keyVaultSecretUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = if (!empty(principalId)) {
  scope: keyVault
  name: guid(resourceGroup().id, principalId, keyVaultSecretUserRoleRoleDefinition.id)
  properties: {
    roleDefinitionId: keyVaultSecretUserRoleRoleDefinition.id
    principalId: principalId
    principalType: 'User'
  }
}

resource dbserver 'Microsoft.Sql/servers@2023-05-01-preview' = {
  location: location
  name: '${appName}-server'
  properties: {
    administratorLogin: '${appName}-server-admin'
    administratorLoginPassword: databasePassword
    publicNetworkAccess: 'Disabled'
    restrictOutboundNetworkAccess: 'Disabled'
  }
  resource db 'databases@2023-05-01-preview' = {
    location: location
    name: '${appName}-database'
    sku: {
      name: 'GP_S_Gen5'
      tier: 'GeneralPurpose'
      family: 'Gen5'
      capacity: 1
    }
  }
}

resource dbPrivateEndpoint 'Microsoft.Network/privateEndpoints@2023-04-01' = {
  name: '${appName}-db-privateEndpoint'
  location: location
  properties: {
    subnet: {
      id: virtualNetwork::subnetForDb.id
    }
    privateLinkServiceConnections: [
      {
        name: '${appName}-db-privateEndpoint'
        properties: {
          privateLinkServiceId: dbserver.id
          groupIds: ['sqlServer']
        }
      }
    ]
  }
  resource dbPrivateDnsZoneGroup 'privateDnsZoneGroups@2024-01-01' = {
    name: 'default'
    properties: {
      privateDnsZoneConfigs: [
        {
          name: 'database-config'
          properties: {
            privateDnsZoneId: privateDnsZoneDB.id
          }
        }
      ]
    }
  }
}

resource redisCache 'Microsoft.Cache/Redis@2023-08-01' = {
  name: '${appName}-cache'
  location: location
  properties: {
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
    redisConfiguration: {}
    enableNonSslPort: false
    redisVersion: '6'
    publicNetworkAccess: 'Disabled'
  }
}

resource cachePrivateEndpoint 'Microsoft.Network/privateEndpoints@2023-04-01' = {
  name: '${appName}-cache-privateEndpoint'
  location: location
  properties: {
    subnet: {
      id: virtualNetwork::subnetForCache.id
    }
    privateLinkServiceConnections: [
      {
        name: '${appName}-cache-privateEndpoint'
        properties: {
          privateLinkServiceId: redisCache.id
          groupIds: ['redisCache']
        }
      }
    ]
  }
  resource privateDnsZoneGroup 'privateDnsZoneGroups' = {
    name: 'default'
    properties: {
      privateDnsZoneConfigs: [
        {
          name: 'cache-config'
          properties: {
            privateDnsZoneId: privateDnsZoneCache.id
          }
        }
      ]
    }
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: '${appName}-workspace'
  location: location
  properties: any({
    retentionInDays: 30
    features: {
      searchVersion: 1
    }
    sku: {
      name: 'PerGB2018'
    }
  })
}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-11-01-preview' = {
  name: containerRegistryName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: '${appName}-cae'
  location: location
  properties: {
    workloadProfiles: [
      {
        name: 'Consumption'
        workloadProfileType: 'Consumption'
      }
    ]
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsWorkspace.properties.customerId
        sharedKey: listKeys(logAnalyticsWorkspace.id, logAnalyticsWorkspace.apiVersion).primarySharedKey
      }
    }
    vnetConfiguration: {
      infrastructureSubnetId: virtualNetwork::subnetForAca.id
      internal: false
    }
  }
}

resource web 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: location
  tags: {
    'azd-service-name': 'web'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 80
        allowInsecure: false
        transport: 'auto'
      }
      registries: [
        {
          server: containerRegistry.properties.loginServer
          username: listCredentials(containerRegistry.id, containerRegistry.apiVersion).username
          passwordSecretRef: 'acr-password'
        }
      ]
      secrets: [
        {
          name: 'acr-password'
          value: listCredentials(containerRegistry.id, containerRegistry.apiVersion).passwords[0].value
        }
        {
          name: 'sql-connection-string'
          value: sqlConnectionString
        }
        {
          name: 'redis-connection-string'
          value: '${redisCache.name}.redis.cache.windows.net:6380,password=${listKeys(redisCache.id, redisCache.apiVersion).primaryKey},ssl=True,abortConnect=False'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'web'
          image: 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Production'
            }
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://+:80'
            }
            {
              name: 'AZURE_SQL_CONNECTIONSTRING'
              secretRef: 'sql-connection-string'
            }
            {
              name: 'AZURE_REDIS_CONNECTIONSTRING'
              secretRef: 'redis-connection-string'
            }
          ]
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 3
      }
    }
  }
  dependsOn: [
    dbPrivateEndpoint
    cachePrivateEndpoint
    vaultPrivateEndpoint
  ]
}

resource acrPullRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: subscription()
  name: '7f951dda-4ed3-4680-a7ca-43fe172d538d'
}

resource acrPullRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: containerRegistry
  name: guid(containerRegistry.id, web.name, acrPullRoleDefinition.id)
  properties: {
    roleDefinitionId: acrPullRoleDefinition.id
    principalId: web.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

output WEB_URI string = 'https://${web.properties.configuration.ingress.fqdn}'
output CONNECTION_SETTINGS array = [
  'AZURE_SQL_CONNECTIONSTRING'
  'AZURE_REDIS_CONNECTIONSTRING'
]
output CONTAINER_APP_NAME string = web.name
output CONTAINER_APP_ID string = web.id
output CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.properties.loginServer
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.properties.loginServer
