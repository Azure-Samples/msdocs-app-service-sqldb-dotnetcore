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

## Getting help

If you're working with this project and running into issues, please post in [Issues](/issues).
## Getting started
