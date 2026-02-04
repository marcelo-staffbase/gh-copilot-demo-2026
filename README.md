# Github Copilot demo 

## Demo Scenarios

### To start discovering Github Copilot jump to [`The Ultimate GitHub Copilot Tutorial on MOAW`](https://aka.ms/github-copilot-hol)
<br/>


## Solution Overview


This repository has been inspired by the [Azure Container Apps: Dapr Albums Sample](https://github.com/Azure-Samples/containerapps-dapralbums)

It's used as a code base to demonstrate Github Copilot capabilities.

The solution is composed of two services: the .net album API and the NodeJS album viewer.


### Album API (`album-api`)

The [`album-api`](./album-api) is an .NET 8 minimal Web API that manage a list of Albums in memory.

### Album Viewer (`album-viewer`)

The [`album-viewer`](./album-viewer) is a modern Vue.js 3 application built with TypeScript through which the albums retrieved by the API are surfaced. The application uses the Vue 3 Composition API with full TypeScript support for enhanced developer experience and type safety. In order to display the repository of albums, the album viewer contacts the backend album API.

## Getting Started

There are multiple ways to run this solution locally. Choose the method that best fits your development workflow.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (version 16 or higher)
- [TypeScript](https://www.typescriptlang.org/) (automatically installed with project dependencies)
- [Visual Studio Code](https://code.visualstudio.com/) (recommended)

### Option 1: Using VS Code Debug Panel (Recommended)

This is the easiest way to run the solution with full debugging capabilities.

1. Open the solution in Visual Studio Code
2. Open the Debug panel (Ctrl+Shift+D / Cmd+Shift+D)
3. Select **"All services"** from the dropdown
4. Click the green play button or press F5

This will automatically:
- Build the .NET API and start it on `http://localhost:3000`
- Start the Vue.js TypeScript app on `http://localhost:3001`
- Open both services in your default browser

You can also run individual services:
- **"C#: Album API Debug"** - Runs only the .NET API
- **"Node.js: Album Viewer Debug"** - Runs only the Vue.js TypeScript frontend

### Option 2: Command Line

#### Starting the Album API (.NET)

```powershell
# Navigate to the API directory
cd albums-api

# Restore dependencies (first time only)
dotnet restore

# Run the API
dotnet run
```

The API will start on `http://localhost:3000` and you can access the Swagger documentation at `http://localhost:3000/swagger`.

#### Starting the Album Viewer (Vue.js + TypeScript)

```powershell
# Navigate to the viewer directory
cd album-viewer

# Install dependencies (first time only)
npm install

# Start the development server
npm run dev

# Optional: Run TypeScript type checking
npm run type-check
```

The Vue.js TypeScript app will start on `http://localhost:3001` and automatically open in your browser.

#### Running Both Services

You can run both services simultaneously using separate terminal windows:

```powershell
# Terminal 1 - Start the API
cd albums-api
dotnet run

# Terminal 2 - Start the Vue TypeScript app
cd album-viewer
npm run dev
```

### Environment Configuration

The solution uses the following default configuration:

- **Album API**: Runs on `http://localhost:3000`
- **Album Viewer**: Runs on `http://localhost:3001` (TypeScript + Vue 3)
- **API Endpoint**: The Vue app is configured to call the API at `localhost:3000`

If you need to change these settings, you can modify:
- API port: `albums-api/Properties/launchSettings.json`
- Vue app configuration: Environment variables in `.vscode/launch.json` or set `VITE_ALBUM_API_HOST` environment variable

### Alternative: GitHub Codespaces

The easiest way is to open this solution in a GitHub Codespace, or run it locally in a devcontainer. The development environment will be automatically configured for you.

## Deploying to Azure

This solution can be deployed to Azure using either Azure Bicep or Terraform. The Infrastructure as Code (IaC) templates are provided in the `iac` directory.

### Prerequisites for Azure Deployment

- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) installed and configured
- An active Azure subscription
- [Docker](https://www.docker.com/get-started) installed (for building container images)
- Appropriate Azure permissions to create resources

### Option 1: Deploy with Azure Bicep

Azure Bicep provides a declarative way to deploy Azure resources. The deployment will create:
- Azure Container Registry (ACR)
- Azure Container Apps Environment
- Log Analytics Workspace
- Application Insights
- Storage Account with Blob Container (for Dapr state store)
- Azure OpenAI Service
- Two Container Apps (Album API and Album Viewer)

#### Step 1: Login to Azure

```bash
az login
az account set --subscription <your-subscription-id>
```

#### Step 2: Create a Resource Group

```bash
az group create --name rg-album-demo --location eastus
```

#### Step 3: Build and Push Container Images

First, create the Azure Container Registry:

```bash
# Create ACR
az acr create --resource-group rg-album-demo \
  --name <your-acr-name> \
  --sku Basic \
  --admin-enabled true

# Login to ACR
az acr login --name <your-acr-name>

# Build and push Album API image
cd albums-api
az acr build --registry <your-acr-name> \
  --image album-api:latest \
  --file Dockerfile .

# Build and push Album Viewer image
cd ../album-viewer
docker build -t <your-acr-name>.azurecr.io/album-viewer:latest .
docker push <your-acr-name>.azurecr.io/album-viewer:latest
```

#### Step 4: Get ACR Credentials

```bash
# Get ACR credentials
ACR_USERNAME=$(az acr credential show --name <your-acr-name> --query "username" -o tsv)
ACR_PASSWORD=$(az acr credential show --name <your-acr-name> --query "passwords[0].value" -o tsv)
```

#### Step 5: Deploy with Bicep

```bash
cd iac/bicep

az deployment group create \
  --resource-group rg-album-demo \
  --template-file main.bicep \
  --parameters location=eastus \
  --parameters registryName=<your-acr-name>.azurecr.io \
  --parameters registryUsername=$ACR_USERNAME \
  --parameters registryPassword=$ACR_PASSWORD \
  --parameters apiImage=<your-acr-name>.azurecr.io/album-api:latest \
  --parameters viewerImage=<your-acr-name>.azurecr.io/album-viewer:latest
```

#### Step 6: Get Application URLs

```bash
# Get Album Viewer URL
az containerapp show \
  --name album-viewer \
  --resource-group rg-album-demo \
  --query properties.configuration.ingress.fqdn \
  -o tsv

# Get Album API URL
az containerapp show \
  --name album-api \
  --resource-group rg-album-demo \
  --query properties.configuration.ingress.fqdn \
  -o tsv
```

### Option 2: Deploy with Terraform

Terraform provides an alternative Infrastructure as Code approach.

#### Step 1: Initialize Terraform

```bash
cd iac/terraform

# Initialize Terraform
terraform init
```

#### Step 2: Create Terraform Variables

Create a `terraform.tfvars` file with your configuration:

```hcl
location = "eastus"
resource_group_name = "rg-album-demo"
container_registry_name = "acralbumdemo"
```

#### Step 3: Plan and Apply

```bash
# Preview changes
terraform plan

# Apply the configuration
terraform apply
```

#### Step 4: Get Outputs

```bash
# View all outputs
terraform output

# Get specific resource information
terraform output container_registry_login_server
terraform output resource_group_name
```

### Post-Deployment Configuration

After deployment, you may need to configure:

1. **CORS Settings**: Ensure the API allows requests from the viewer's domain
2. **Environment Variables**: Update any environment-specific configuration
3. **Custom Domains**: Optionally configure custom domains for your Container Apps
4. **SSL Certificates**: Container Apps provide automatic SSL by default

### Monitoring and Logs

Monitor your deployed application using Azure Portal:

1. **Application Insights**: View telemetry, performance metrics, and failures
   ```bash
   az monitor app-insights component show \
     --resource-group rg-album-demo \
     --app appinsights-<suffix>
   ```

2. **Container App Logs**: Stream logs from your containers
   ```bash
   # Stream Album API logs
   az containerapp logs show \
     --name album-api \
     --resource-group rg-album-demo \
     --follow

   # Stream Album Viewer logs
   az containerapp logs show \
     --name album-viewer \
     --resource-group rg-album-demo \
     --follow
   ```

3. **Log Analytics**: Query logs using Kusto Query Language (KQL)
   ```bash
   az monitor log-analytics workspace show \
     --resource-group rg-album-demo \
     --workspace-name log-<suffix>
   ```

### Scaling

Azure Container Apps support automatic scaling based on HTTP traffic:

```bash
# Update scaling rules for Album API
az containerapp update \
  --name album-api \
  --resource-group rg-album-demo \
  --min-replicas 1 \
  --max-replicas 10

# Update scaling rules for Album Viewer
az containerapp update \
  --name album-viewer \
  --resource-group rg-album-demo \
  --min-replicas 1 \
  --max-replicas 5
```

### Clean Up Resources

To avoid Azure charges, delete the resource group when you're done:

```bash
az group delete --name rg-album-demo --yes --no-wait
```

Or if using Terraform:

```bash
cd iac/terraform
terraform destroy
```

### Troubleshooting Deployment Issues

**Container fails to start:**
- Check container logs using `az containerapp logs show`
- Verify ACR credentials are correct
- Ensure container images were built successfully

**Can't access the application:**
- Verify ingress is enabled on the Container App
- Check if the Container App is running: `az containerapp show`
- Review Application Insights for errors

**Authentication errors:**
- Verify ACR credentials match the ones used in deployment
- Check if the Container App has permission to pull from ACR

### CI/CD Integration

For automated deployments, consider setting up:
- **GitHub Actions**: Use the provided workflows in `.github/workflows`
- **Azure DevOps**: Create pipelines for automated build and deployment
- **Dapr Integration**: The infrastructure supports Dapr for microservices patterns

For more information on Azure Container Apps, visit the [official documentation](https://learn.microsoft.com/azure/container-apps/).