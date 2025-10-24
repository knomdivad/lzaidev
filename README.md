# Azure AI Landing Zone

A comprehensive, enterprise-ready AI development platform built on Azure cloud infrastructure. This landing zone provides a secure, scalable, and cost-effective foundation for AI/ML workloads with built-in monitoring, governance, and operational tools.

## 🏗️ Architecture Overview

The AI Landing Zone consists of:

### Infrastructure Components
- **Azure Kubernetes Service (AKS)** - Container orchestration with dedicated AI workload node pools
- **Azure Machine Learning** - Comprehensive ML workspace for model development and deployment
- **Azure OpenAI Service** - Access to GPT-4, GPT-3.5 Turbo, and embedding models
- **Azure Cognitive Services** - Pre-built AI services for common scenarios
- **Azure AI Search** - Intelligent search capabilities for knowledge bases
- **Azure Storage** - Data Lake Gen2 for AI datasets and model storage
- **Azure Container Registry** - Private container image registry
- **Azure Key Vault** - Secure secrets and encryption key management
- **Azure Monitor & Application Insights** - Comprehensive observability

### Management Portal
- **.NET 8 Web API** - RESTful API for infrastructure management
- **React Frontend** (planned) - User-friendly dashboard for monitoring and deployment
- **Cost Management** - Real-time cost tracking and optimization recommendations
- **Deployment Automation** - One-click infrastructure deployment via Terraform

## 🚀 Quick Start

### Prerequisites
- Docker and Docker Compose
- Azure subscription with Contributor access (for cloud deployment)
- Terraform >= 1.5.0 (for cloud deployment)
- Azure CLI >= 2.50.0 (for cloud deployment)

### 1. Local Development (Docker)

The fastest way to get started is using the containerized development environment:

```bash
# Clone the repository
git clone https://github.com/knomdivad/lzaidev.git
cd lzaidev

# Start in development mode (with hot reload)
make dev

# Check service health
make health

# View logs
make logs

# Stop services
make stop
```

**Services will be available at:**
- Frontend: http://localhost:3000
- Backend API: http://localhost:5001/api
- API Documentation: http://localhost:5001/swagger
- Redis: localhost:6379

**⚠️ macOS Users:** Port 5000 is reserved by AirPlay. The backend uses port 5001 instead.

### Available Docker Commands

```bash
make dev        # Start in development mode (hot reload)
make prod       # Start in production mode  
make stop       # Stop all services
make health     # Check service health
make logs       # View service logs
make clean      # Stop and clean volumes
make rebuild    # Rebuild and start in dev mode
make status     # Show service status
```

### 2. Infrastructure Deployment (Cloud)

```bash
# Clone the repository
git clone https://github.com/knomdivad/lzaidev.git
cd lzaidev

# Initialize Terraform
cd terraform
terraform init

# Plan deployment
terraform plan -var="project_name=my-ai-project" -var="environment=dev" -var="location=eastus"

# Deploy infrastructure
terraform apply -var="project_name=my-ai-project" -var="environment=dev" -var="location=eastus"
```

### 2. Portal Deployment

```bash
# Build and run the management portal
cd src/LandingZoneAI.Portal
dotnet run

# Or using Docker
docker build -t landing-zone-ai-portal .
docker run -p 8080:8080 landing-zone-ai-portal
```

### 3. Configure Azure Authentication

```bash
# Create service principal for the portal
az ad sp create-for-rbac --name "landing-zone-ai-portal" --role contributor --scopes /subscriptions/{subscription-id}

# Set environment variables or update appsettings.json
export AZURE_CLIENT_ID="your-client-id"
export AZURE_CLIENT_SECRET="your-client-secret"
export AZURE_TENANT_ID="your-tenant-id"
export AZURE_SUBSCRIPTION_ID="your-subscription-id"
```

## 📊 Features

### Infrastructure Management
- **Multi-environment support** (dev, staging, production)
- **Infrastructure as Code** with Terraform modules
- **Network security** with private endpoints and NSGs
- **Auto-scaling** AKS clusters with GPU node pools
- **Disaster recovery** with geo-redundant storage

### AI/ML Capabilities
- **Azure OpenAI** with pre-deployed GPT models
- **MLOps integration** with Azure ML pipelines
- **Model registry** for versioning and governance
- **Experiment tracking** and model monitoring
- **Vector search** with Azure AI Search

### Monitoring & Governance
- **Real-time cost tracking** and budget alerts
- **Performance monitoring** with Azure Monitor
- **Security compliance** with Key Vault integration
- **Audit logging** for all operations
- **Resource governance** with Azure Policy

### Developer Experience
- **REST API** for programmatic access
- **CI/CD pipelines** with GitHub Actions
- **Container-ready** with AKS deployment
- **Documentation** with OpenAPI/Swagger
- **Local development** support

## 🔧 Configuration

### Terraform Variables

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `project_name` | Name of the project for resource naming | - | Yes |
| `environment` | Environment (dev/staging/prod) | - | Yes |
| `location` | Azure region | `eastus` | Yes |
| `enable_private_endpoints` | Enable private networking | `true` | No |
| `aks_node_count` | Number of system nodes | `3` | No |
| `aks_vm_size` | VM size for system nodes | `Standard_D4s_v3` | No |
| `ai_node_count` | Number of AI workload nodes | `2` | No |
| `ai_vm_size` | VM size for AI nodes | `Standard_NC4as_T4_v3` | No |

### Portal Configuration

Key configuration options in `appsettings.json`:

```json
{
  "Azure": {
    "DefaultSubscriptionId": "your-subscription-id",
    "TenantId": "your-tenant-id"
  },
  "Features": {
    "EnableAuthentication": true,
    "EnableCostTracking": true,
    "EnableTelemetry": true
  }
}
```

## 📁 Project Structure

```
lzaidev/
├── terraform/                 # Infrastructure as Code
│   ├── modules/               # Reusable Terraform modules
│   │   ├── networking/        # VNet, subnets, NSGs
│   │   ├── security/          # Key Vault, RBAC
│   │   ├── storage/           # Storage accounts
│   │   ├── container_registry/# Azure Container Registry
│   │   ├── monitoring/        # Log Analytics, App Insights
│   │   ├── aks/              # Kubernetes cluster
│   │   └── ai_services/       # ML, OpenAI, Cognitive Services
│   ├── main.tf               # Root configuration
│   ├── variables.tf          # Input variables
│   └── outputs.tf            # Output values
├── src/                      # Application source code
│   └── LandingZoneAI.Portal/ # .NET management portal
│       ├── Controllers/      # API controllers
│       ├── Services/         # Business logic
│       ├── Models/           # Data models
│       └── Program.cs        # Application entry point
├── k8s/                      # Kubernetes manifests
│   ├── deployment.yaml       # Portal deployment
│   └── rbac.yaml            # RBAC configuration
├── .github/workflows/        # CI/CD pipelines
│   ├── terraform.yml         # Infrastructure pipeline
│   └── dotnet-portal.yml     # Portal pipeline
└── docs/                     # Documentation
```

## 🔒 Security

### Network Security
- Private endpoints for all PaaS services
- Network Security Groups with least-privilege rules
- Azure Firewall for egress traffic control
- Private DNS zones for internal name resolution

### Identity & Access
- Azure AD integration for authentication
- RBAC for fine-grained permissions
- Managed identities for service-to-service auth
- Key Vault for secrets management

### Data Protection
- Encryption at rest and in transit
- Customer-managed keys in Key Vault
- Data loss prevention policies
- Backup and disaster recovery

## 💰 Cost Management

### Cost Optimization Features
- **Real-time cost tracking** by resource and tag
- **Budget alerts** when spending exceeds thresholds
- **Resource recommendations** for rightsizing
- **Auto-shutdown** policies for dev/test environments
- **Reserved instance** recommendations

### Typical Monthly Costs (East US)

| Service | SKU | Estimated Cost |
|---------|-----|----------------|
| AKS Cluster | 3x Standard_D4s_v3 | $350 |
| AI Node Pool | 2x Standard_NC4as_T4_v3 | $600 |
| Azure OpenAI | GPT-4 (10K tokens) | $200 |
| ML Workspace | Standard | $100 |
| Storage | 1TB Data Lake | $50 |
| Container Registry | Premium | $25 |
| Key Vault | Standard | $5 |
| **Total** | | **~$1,330** |

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/new-feature`)
3. Commit your changes (`git commit -m 'Add new feature'`)
4. Push to the branch (`git push origin feature/new-feature`)
5. Open a Pull Request

## 📖 Documentation

- [API Documentation](./docs/api.md)
- [Deployment Guide](./docs/deployment.md)
- [Architecture Decision Records](./docs/adr/)
- [Troubleshooting](./docs/troubleshooting.md)

## 🆘 Support

- **Issues**: [GitHub Issues](https://github.com/knomdivad/lzaidev/issues)
- **Discussions**: [GitHub Discussions](https://github.com/knomdivad/lzaidev/discussions)
- **Documentation**: [Wiki](https://github.com/knomdivad/lzaidev/wiki)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🎯 Roadmap

- [x] Core infrastructure deployment
- [x] Management portal API
- [ ] React frontend dashboard
- [ ] Cost optimization recommendations
- [ ] Multi-region deployment
- [ ] Azure Policy integration
- [ ] Prometheus/Grafana monitoring
- [ ] Jupyter Hub integration
