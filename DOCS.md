# GitHub Copilot Documentation

This documentation is generated with GitHub Copilot to show what the tool can do.

## Overview

This project demonstrates GitHub Copilot's capabilities through a full-stack album management application. The solution consists of two main components:

- **Album API**: A .NET 8 minimal Web API for managing albums
- **Album Viewer**: A Vue.js 3 + TypeScript frontend application

## Architecture

### Album API (Backend)

The album API is built with .NET 8 and provides RESTful endpoints for album management.

**Key Features:**
- In-memory album storage
- RESTful API endpoints
- Swagger documentation at `/swagger`
- Support for sorting albums by title, artist, or price
- CORS enabled for frontend integration

**Main Endpoints:**
- `GET /albums` - Retrieve all albums with optional sorting
  - Query parameter: `sortBy` (values: "title", "artist", "price")
- `GET /albums/{id}` - Get album details by ID

**Technologies:**
- .NET 8
- ASP.NET Core Minimal API
- Swagger/OpenAPI

### Album Viewer (Frontend)

The album viewer is a modern single-page application built with Vue.js 3 and TypeScript.

**Key Features:**
- Vue 3 Composition API with TypeScript
- Responsive grid layout for album display
- Real-time data fetching from the API
- Loading states and error handling
- D3.js visualization for album sales data
- Album card components with album details

**Technologies:**
- Vue.js 3
- TypeScript
- Vite (build tool)
- Axios (HTTP client)
- D3.js (data visualization)

## Project Structure

```
gh-copilot-demo-2026/
├── albums-api/              # .NET 8 Web API
│   ├── Controllers/         # API controllers
│   │   ├── AlbumController.cs
│   │   └── UnsecuredController.cs
│   ├── Models/             # Data models
│   │   └── Album.cs
│   ├── Program.cs          # Application entry point
│   └── albums-api.csproj   # Project configuration
│
├── album-viewer/           # Vue.js + TypeScript frontend
│   ├── src/
│   │   ├── components/    # Vue components
│   │   │   └── AlbumCard.vue
│   │   ├── types/         # TypeScript type definitions
│   │   │   └── album.ts
│   │   ├── utils/         # Utility functions
│   │   │   ├── validators.ts
│   │   │   └── viz.ts     # D3.js visualizations
│   │   ├── App.vue        # Main application component
│   │   └── main.ts        # Application entry point
│   ├── package.json       # NPM dependencies
│   └── vite.config.ts     # Vite configuration
│
├── iac/                   # Infrastructure as Code
│   ├── bicep/            # Azure Bicep templates
│   │   ├── main.bicep
│   │   └── modules/
│   └── terraform/        # Terraform configurations
│       └── apps.tf
│
└── legacy/               # Legacy code samples
    └── albums.cbl        # COBOL sample
```

## Getting Started

### Prerequisites

Ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (version 16 or higher)
- [Visual Studio Code](https://code.visualstudio.com/) (recommended)

### Running the Application

#### Option 1: Using VS Code Debug Panel (Recommended)

1. Open the solution in Visual Studio Code
2. Open the Debug panel (Ctrl+Shift+D / Cmd+Shift+D)
3. Select **"All services"** from the dropdown
4. Click the green play button or press F5

This will start both services:
- Album API on `http://localhost:3000`
- Album Viewer on `http://localhost:3001`

#### Option 2: Using VS Code Tasks

The project includes pre-configured VS Code tasks for easy execution:

1. Open the Command Palette (Ctrl+Shift+P / Cmd+Shift+P)
2. Type "Tasks: Run Task"
3. Select one of the available tasks:
   - **build** - Build the .NET API
   - **publish** - Publish the .NET API
   - **watch** - Run the solution with hot-reload
   - **npm: install - album-viewer** - Install frontend dependencies
   - **shell: npm: dev - album-viewer** - Start the Vue.js development server

#### Option 3: Command Line

**Starting the Album API:**

```bash
cd albums-api
dotnet restore
dotnet run
```

The API will be available at `http://localhost:3000` with Swagger UI at `http://localhost:3000/swagger`.

**Starting the Album Viewer:**

```bash
cd album-viewer
npm install
npm run dev
```

The viewer will be available at `http://localhost:3001`.

## Development

### Album API Development

The Album API uses ASP.NET Core controllers to handle HTTP requests.

**Adding a New Endpoint:**

```csharp
// In AlbumController.cs
[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    var album = Album.GetById(id);
    if (album == null)
        return NotFound();
    return Ok(album);
}
```

**Building the API:**

```bash
cd albums-api
dotnet build
```

**Running Tests:**

```bash
dotnet test
```

### Album Viewer Development

The Album Viewer uses Vue 3's Composition API with TypeScript for type safety.

**Component Structure:**

Components are located in `album-viewer/src/components/`. Each component uses the `<script setup>` syntax with TypeScript.

**Type Checking:**

```bash
cd album-viewer
npm run type-check
```

**Running Tests:**

```bash
npm run test
```

**Building for Production:**

```bash
npm run build
```

### Data Visualization

The project includes D3.js-based visualizations for album sales data in `album-viewer/src/utils/viz.ts`.

**Using the Visualization:**

```typescript
import { createAlbumSalesChart } from '@/utils/viz';

// Render chart
await createAlbumSalesChart('chart-container', '/api/sales-data.json');
```

**Expected Data Format:**

```json
[
  { "month": "Jan", "albumsSold": 150, "year": 2024 },
  { "month": "Feb", "albumsSold": 200, "year": 2024 }
]
```

## Infrastructure as Code

### Azure Bicep

The `iac/bicep/` directory contains Azure Bicep templates for deploying the solution to Azure.

**Resources Defined:**
- Container Apps Environment
- Log Analytics Workspace
- Application Insights
- Storage Account with Blob Container
- Container Registry
- Azure OpenAI Service
- Dapr State Store

**Deploying with Bicep:**

```bash
cd iac/bicep
az deployment group create \
  --resource-group <your-rg> \
  --template-file main.bicep \
  --parameters registryName=<registry> \
               registryUsername=<username> \
               registryPassword=<password> \
               apiImage=<api-image> \
               viewerImage=<viewer-image>
```

### Terraform

The `iac/terraform/` directory contains Terraform configurations for infrastructure provisioning.

**Resources Defined:**
- SQL Database with schema initialization
- Container Registry
- Azure OpenAI Service
- Docker image builds for various services

**Deploying with Terraform:**

```bash
cd iac/terraform
terraform init
terraform plan
terraform apply
```

## Environment Variables

### Album Viewer

Configure the following environment variables for the Vue.js app:

- `VITE_ALBUM_API_HOST` - Album API host (default: `localhost:3000`)
- `VITE_BACKGROUND_COLOR` - Background color theme (default: `black`)

Set these in `.vscode/launch.json` or create a `.env` file:

```env
VITE_ALBUM_API_HOST=localhost:3000
VITE_BACKGROUND_COLOR=black
```

### Album API

Configure in `albums-api/appsettings.json` or `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

## Docker Support

The Album API includes a Dockerfile for containerization.

**Building the Docker Image:**

```bash
cd albums-api
docker build -t album-api:latest .
```

**Running in Docker:**

```bash
docker run -p 3000:80 album-api:latest
```

## GitHub Codespaces

The easiest way to get started is using GitHub Codespaces:

1. Click the "Code" button on the GitHub repository
2. Select "Open with Codespaces"
3. Click "New codespace"

The development environment will be automatically configured with all dependencies.

## Troubleshooting

### API Not Starting

- Ensure .NET 8 SDK is installed: `dotnet --version`
- Check if port 3000 is available
- Review logs in the terminal for error details

### Frontend Not Starting

- Ensure Node.js is installed: `node --version`
- Check if port 3001 is available
- Clear node_modules and reinstall: `rm -rf node_modules && npm install`

### TypeScript Errors

- Run type checking: `npm run type-check`
- Ensure all dependencies are installed: `npm install`
- Check for conflicting TypeScript versions

### CORS Errors

- Verify the API allows CORS from the frontend origin
- Check the `VITE_ALBUM_API_HOST` environment variable
- Ensure the API is running before starting the frontend

## Additional Resources

- [GitHub Copilot Tutorial](https://aka.ms/github-copilot-hol)
- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [Vue.js 3 Documentation](https://vuejs.org/)
- [.NET 8 Documentation](https://learn.microsoft.com/dotnet/)