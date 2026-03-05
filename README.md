---
languages:
- csharp
- aspx-csharp
- bicep
page_type: sample
products:
- azure
- aspnet-core
- azure-app-service
- azure-sql-database
- azure-virtual-network
urlFragment: msdocs-app-service-sqldb-dotnetcore
name: Deploy an ASP.NET Core web app with SQL Database in Azure
description: "A sample application you can use to follow along with Tutorial: Deploy an ASP.NET Core and Azure SQL Database app to Azure App Service."
---

# Deploy an ASP.NET Core web app with SQL Database in Azure

This is an ASP.NET Core application that you can use to follow along with the tutorial at 
[Tutorial: Deploy an ASP.NET Core and Azure SQL Database app to Azure App Service](https://learn.microsoft.com/azure/app-service/tutorial-dotnetcore-sqldb-app) or by using the [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/overview) according to the instructions below.

## Run the sample

This project has a [dev container configuration](.devcontainer/), which makes it easier to develop apps locally, deploy them to Azure, and monitor them. The easiest way to run this sample application is inside a GitHub codespace. Follow these steps:

1. Fork this repository to your account. For instructions, see [Fork a repo](https://docs.github.com/get-started/quickstart/fork-a-repo).

1. From the repository root of your fork, select **Code** > **Codespaces** > **+**.

1. In the codespace terminal, run the following commands:

    ```shell
    dotnet ef database update
    dotnet run
    ```

1. When you see the message `Your application running on port 5093 is available.`, click **Open in Browser**.

## Run with Docker (local SQL + Redis)

From the repository root:

```shell
docker compose up --build
```

Then open:

```text
http://localhost:8080
```

The `web` container runs EF migrations at startup, then launches the app. SQL Server and Redis are started by `docker-compose.yml`.

## What changed in this branch

This branch migrated the AZD deployment target from **Azure App Service** to **Azure Container Apps (ACA)** and kept local Docker workflow available.

- `azure.yaml`: service host changed to `containerapp`, Docker registry is explicitly configured.
- `infra/resources.bicep`: compute resources changed from `Microsoft.Web/*` to `Microsoft.App/*` and `Microsoft.ContainerRegistry/*`.
- `infra/main.bicep`: outputs aligned to ACA deployment (container app and registry endpoint).
- `Dockerfile` + `docker-compose.yml` + `docker-entrypoint.sh`: local containerized workflow added (web + SQL Server + Redis).

## Run modes (4 scenarios)

| Mode | Main command | Compute | Data/cache | Notes |
|---|---|---|---|---|
| Local (.NET) | `dotnet run` | Local process | Local SQL (from `ConnectionStrings:MyDbConnection`), local memory cache in Development | Fastest for debugging code only. |
| Azure App Service | `azd up` (on App Service branch/template) | App Service (Linux Web App) | Azure SQL + Azure Redis + Key Vault (private endpoints) | Legacy deployment path from original template. |
| Local Docker | `docker compose up --build` | Local Docker container | Local SQL Server container + local Redis container | Closer to cloud runtime than plain `dotnet run`. |
| Azure Container Apps | `azd up -e <env>` (current branch) | Azure Container Apps | Azure SQL + Azure Redis + Key Vault (private endpoints) + ACR image pull | Current deployment path in this branch. |

### Recommended commands by mode

Local (.NET):

```shell
dotnet ef database update
dotnet run
```

Local Docker:

```shell
docker compose up --build
```

Azure Container Apps (current branch):

```shell
azd env new VSCodeFirstDemoAca
azd up -e VSCodeFirstDemoAca --no-prompt
```

Update ACA after code change:

```shell
azd deploy -e VSCodeFirstDemoAca --no-prompt
```

## ACA deployment troubleshooting (real issues and fixes)

These are issues encountered during the actual migration/deployment workflow and how they were resolved.

### 1) `ContainerAppInvalidName` (name must be lowercase)

Symptom:
- `Invalid ContainerApp name ... must consist of lower case ...`

Root cause:
- Environment name contained uppercase letters, and generated Container App name reused it directly.

Fix:
- Keep shared resource naming as-is, but use a dedicated lowercase variable for Container App name only:
  - `var containerAppName = toLower(appName)`
  - `web.name = containerAppName`

### 2) Resource provider not registered

Symptom:
- `SubscriptionIsNotRegistered ... Microsoft.App and Microsoft.ContainerService`

Fix:

```shell
az provider register --namespace Microsoft.App
az provider register --namespace Microsoft.ContainerService
```

Wait until both return `Registered`.

### 3) ACA subnet size requirement

Symptom:
- `ManagedEnvironmentInvalidNetworkConfiguration ... subnet must have a size of at least /23`

Root cause:
- ACA environment infrastructure subnet used `/24`.

Fix:
- Use a dedicated ACA infrastructure subnet with at least `/23` (example: `10.0.4.0/23`).

### 4) Existing subnet/environment conflicts

Symptoms:
- `InUseSubnetCannotBeUpdated`
- `InUseSubnetCannotBeDeleted`
- `ManagedEnvironmentInfraSubnetIdCannotBeUpdated`

Root cause:
- Previous failed/partial deployments left resources bound to old subnet settings.

Fix:
- For test environments, reset and redeploy cleanly:

```shell
azd down -e VSCodeFirstDemoAca --force --no-prompt
az keyvault purge --name <soft-deleted-kv-name> --location <region>   # if needed
azd up -e VSCodeFirstDemoAca --no-prompt
```

### 5) Revision provisioning timeout (`Operation expired`)

Symptom:
- `ContainerAppOperationError ... Failed to provision revision ... Operation expired`

Root causes observed:
- Ingress `targetPort` mismatch for initial container image.
- Registry/auth configuration mismatch caused non-starting revisions.

Fixes:
- Align `targetPort` to actual container listening port.
- Ensure container registry settings are valid and deploy path can push/pull image.

### 6) `azd deploy` cannot determine registry endpoint

Symptom:
- `could not determine container registry endpoint ...`

Fix:
- Ensure `AZURE_CONTAINER_REGISTRY_ENDPOINT` is present in environment outputs.
- Set registry explicitly in `azure.yaml`:

```yaml
services:
  web:
    docker:
      path: ./Dockerfile
      registry: ${AZURE_CONTAINER_REGISTRY_ENDPOINT}
```

### 7) ACR image pull unauthorized during revision update

Symptom:
- `UNAUTHORIZED: authentication required` when ACA pulls image from ACR.

Fix:
- Configure ACA registry with explicit credentials (secret + username from ACR credentials), or ensure managed identity pull path is fully wired and propagated before deployment.

### 8) Post-provision hook parsing failure

Symptom:
- PowerShell `ConvertFrom-Json` failed because `CONNECTION_SETTINGS` was null.

Fix:
- Guard the hook:
  - If value exists, parse JSON.
  - Else print `(none)` and continue.

### 9) Still seeing the default "Your Azure Container Apps app is live" page

Symptom:
- ACA endpoint returns the default welcome page instead of the Todo app.

Root cause:
- `azd provision` applies infra template state. If the template uses a bootstrap image for initial provisioning, the Container App image can be reset to that default image.

Fix:
- Run deploy after provision to publish your app image revision:

```shell
azd provision -e VSCodeFirstDemoAca --no-prompt
azd deploy -e VSCodeFirstDemoAca --no-prompt
```

- Verify active revision image:

```shell
az containerapp revision list -g VSCodeFirstDemoAca_group -n vscodefirstdemoaca-koqndqit3fkna -o table
```

### 10) App starts, migrations succeed, but requests still fail with connection string errors

Symptom:
- Runtime error page with `The ConnectionString property has not been initialized.`
- Migrations may still succeed in startup.

Root cause:
- The app uses `builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")`, which maps to configuration key:
  - `ConnectionStrings__AZURE_SQL_CONNECTIONSTRING`
- Only `AZURE_SQL_CONNECTIONSTRING` was set in container env, so EF runtime query path had empty connection string.

Fix:
- Set both env vars in ACA template to the same secret:
  - `AZURE_SQL_CONNECTIONSTRING`
  - `ConnectionStrings__AZURE_SQL_CONNECTIONSTRING`
 
## Quick deploy

This project is designed to work well with the [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/overview), which makes it easier to develop apps locally, deploy them to Azure, and monitor them.

🎥 Watch a deployment of the code in [this screencast](https://www.youtube.com/watch?v=JDlZ4TgPKYc).

In the GitHub codespace:

1. Log in to Azure.

    ```shell
    azd auth login
    ```

1. Provision and deploy all the resources:

    ```shell
    azd up
    ```

    It will prompt you to create a deployment environment name, pick a subscription, and provide a location (like `westeurope`). Then it will provision the resources in your account and deploy the latest code. If you get an error with deployment, changing the location (like to "centralus") can help, as there may be availability constraints for some of the resources.

1. When `azd` has finished deploying, you'll see an endpoint URI in the command output. Visit that URI, and you should see the CRUD app! 🎉 If you see an error, open the Azure Portal from the URL in the command output, navigate to the App Service, select Logstream, and check the logs for any errors.

1. When you've made any changes to the app code, you can just run:

    ```shell
    azd deploy
    ```

## How is database migrations automated?

The [AZD template](infra/resources.bicep) in this repo secures the database in a virtual network through a private endpoint. The web app can access the database through the private endpoint because it's integrated with the virtual network. In this architecture, the simplest way to do database migrations is directly from within the web app itself.

Because the Linux .NET container in App Service doesn't come with the .NET SDK, you cannot run the migrations command `dotnet ef database update` easily. However, you can upload a [self-contained migrations bundle](https://learn.microsoft.com/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#bundles). This repo automates the deployment of the migrations bundle as follows:

- In [azure.yaml](azure.yaml), use the `prepackage` hook to generate a *migrationsbundle* file with `dotnet ef migrations bundle`.
- In the [.csproj](DotNretCoreSqlDb.csproj) file, include the generated *migrationsbundle* file. During the `azd package` stage, *migrationsbundle* will be added to the deploy package.
- In [infra/resources.bicep](infra/resources.bicep), add the `appCommandLine` property to the web app to run the uploaded *migrationsbundle*.

## How does the AZD template configure passwords?

Two types of secrets are involved: the SQL Database administrator password and the access key for Cache for Redis, and they are both present in the respective connection strings. The [AZD template](infra/resources.bicep) in this repo manages both connection strings in a key vault that's secured behind a private endpoint.

To simplify the scenario, the AZD template generates a new database password each time you run `azd provision` or `azd up`, and the database connection string in the key vault is modified too. If you want to fully utilize `secretOrRandomPassword` in the [parameter file](infra/main.parameters.json) by committing the automatically generated password to the key vault the first time and reading it on subsequent `azd` commands, you must relax the networking restriction of the key vault to allow traffic from public networks. For more information, see [What is the behavior of the `secretOrRandomPassword` function?](https://learn.microsoft.com/azure/developer/azure-developer-cli/faq#what-is-the-behavior-of-the--secretorrandompassword--function).

## Getting help

If you're working with this project and running into issues, please post in [Issues](/issues).
