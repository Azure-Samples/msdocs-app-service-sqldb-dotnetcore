// existing SqlServer params
param sqlServerName string = 'devopsdemos'
param location string = resourceGroup().location

//new sql database params
param sqlDatabaseName string = 'dotnetcoreapp'

resource sqlServer 'Microsoft.Sql/servers@2022-02-01-preview' existing = {
  name: sqlServerName
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-02-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  sku:{
    name: 'Standard'
    tier: 'Standard'
  }
}
