using LandingZoneAI.Portal.Models;

namespace LandingZoneAI.Portal.Services;

/// <summary>
/// Mock implementation of Azure Resource Service for local development and testing
/// </summary>
public class MockAzureResourceService : IAzureResourceService
{
    private readonly ILogger<MockAzureResourceService> _logger;

    public MockAzureResourceService(ILogger<MockAzureResourceService> logger)
    {
        _logger = logger;
    }

    public async Task<List<ResourceSummary>> GetResourcesAsync(string subscriptionId, string? resourceGroupName = null)
    {
        await Task.Delay(500); // Simulate API call delay
        
        _logger.LogInformation("Mock: Getting resources for subscription {SubscriptionId}, RG: {ResourceGroup}", 
            subscriptionId, resourceGroupName ?? "all");

        var mockResources = new List<ResourceSummary>
        {
            new ResourceSummary
            {
                Id = "/subscriptions/mock-sub-123/resourceGroups/rg-lzai-dev/providers/Microsoft.MachineLearningServices/workspaces/mlw-lzai-dev",
                Name = "mlw-lzai-dev",
                Type = "Microsoft.MachineLearningServices/workspaces",
                Location = "East US",
                ResourceGroup = "rg-lzai-dev",
                Status = "Active",
                Tags = new Dictionary<string, string> { ["Environment"] = "dev", ["Project"] = "lzai" }
            },
            new ResourceSummary
            {
                Id = "/subscriptions/mock-sub-123/resourceGroups/rg-lzai-dev/providers/Microsoft.CognitiveServices/accounts/cog-lzai-dev",
                Name = "cog-lzai-dev",
                Type = "Microsoft.CognitiveServices/accounts",
                Location = "East US",
                ResourceGroup = "rg-lzai-dev",
                Status = "Active",
                Tags = new Dictionary<string, string> { ["Environment"] = "dev", ["Project"] = "lzai" }
            },
            new ResourceSummary
            {
                Id = "/subscriptions/mock-sub-123/resourceGroups/rg-lzai-dev/providers/Microsoft.ContainerService/managedClusters/aks-lzai-dev",
                Name = "aks-lzai-dev",
                Type = "Microsoft.ContainerService/managedClusters",
                Location = "East US",
                ResourceGroup = "rg-lzai-dev",
                Status = "Running",
                Tags = new Dictionary<string, string> { ["Environment"] = "dev", ["Project"] = "lzai" }
            },
            new ResourceSummary
            {
                Id = "/subscriptions/mock-sub-123/resourceGroups/rg-lzai-dev/providers/Microsoft.Storage/storageAccounts/stlzaidev123",
                Name = "stlzaidev123",
                Type = "Microsoft.Storage/storageAccounts",
                Location = "East US",
                ResourceGroup = "rg-lzai-dev",
                Status = "Available",
                Tags = new Dictionary<string, string> { ["Environment"] = "dev", ["Project"] = "lzai" }
            },
            new ResourceSummary
            {
                Id = "/subscriptions/mock-sub-123/resourceGroups/rg-lzai-dev/providers/Microsoft.KeyVault/vaults/kv-lzai-dev",
                Name = "kv-lzai-dev",
                Type = "Microsoft.KeyVault/vaults",
                Location = "East US",
                ResourceGroup = "rg-lzai-dev",
                Status = "Active",
                Tags = new Dictionary<string, string> { ["Environment"] = "dev", ["Project"] = "lzai" }
            },
            new ResourceSummary
            {
                Id = "/subscriptions/mock-sub-123/resourceGroups/rg-lzai-dev/providers/Microsoft.ContainerRegistry/registries/acrlzaidev",
                Name = "acrlzaidev",
                Type = "Microsoft.ContainerRegistry/registries",
                Location = "East US",
                ResourceGroup = "rg-lzai-dev",
                Status = "Active",
                Tags = new Dictionary<string, string> { ["Environment"] = "dev", ["Project"] = "lzai" }
            }
        };

        if (!string.IsNullOrEmpty(resourceGroupName))
        {
            mockResources = mockResources.Where(r => 
                r.ResourceGroup.Equals(resourceGroupName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return mockResources;
    }

    public async Task<List<string>> GetResourceGroupsAsync(string subscriptionId)
    {
        await Task.Delay(300); // Simulate API call delay
        
        _logger.LogInformation("Mock: Getting resource groups for subscription {SubscriptionId}", subscriptionId);

        return new List<string>
        {
            "rg-lzai-dev",
            "rg-lzai-staging", 
            "rg-lzai-prod",
            "rg-shared-services",
            "rg-networking"
        };
    }

    public async Task<MLWorkspaceInfo?> GetMLWorkspaceInfoAsync(string subscriptionId, string resourceGroupName, string workspaceName)
    {
        await Task.Delay(400); // Simulate API call delay
        
        _logger.LogInformation("Mock: Getting ML workspace {WorkspaceName} info", workspaceName);

        return new MLWorkspaceInfo
        {
            Id = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.MachineLearningServices/workspaces/{workspaceName}",
            Name = workspaceName,
            Location = "East US",
            DiscoveryUrl = $"https://{workspaceName}.api.azureml.ms/discovery",
            WorkspaceUrl = $"https://ml.azure.com/workspaces/{workspaceName}",
            Status = "Active",
            ComputeInstances = new List<ComputeInstance>
            {
                new ComputeInstance
                {
                    Name = "ci-dev-instance-01",
                    Type = "ComputeInstance",
                    State = "Running",
                    VmSize = "Standard_DS3_v2",
                    CreatedTime = DateTime.UtcNow.AddDays(-5)
                },
                new ComputeInstance
                {
                    Name = "ci-gpu-instance-01",
                    Type = "ComputeInstance", 
                    State = "Stopped",
                    VmSize = "Standard_NC6s_v3",
                    CreatedTime = DateTime.UtcNow.AddDays(-2)
                }
            },
            Models = new List<MLModel>
            {
                new MLModel
                {
                    Name = "sentiment-classifier",
                    Version = "1.2.0",
                    Framework = "scikit-learn",
                    CreatedTime = DateTime.UtcNow.AddDays(-10),
                    Description = "Text sentiment classification model"
                },
                new MLModel
                {
                    Name = "image-detector",
                    Version = "2.1.0", 
                    Framework = "PyTorch",
                    CreatedTime = DateTime.UtcNow.AddDays(-3),
                    Description = "Object detection model for images"
                }
            }
        };
    }

    public async Task<OpenAIServiceInfo?> GetOpenAIServiceInfoAsync(string subscriptionId, string resourceGroupName, string serviceName)
    {
        await Task.Delay(350); // Simulate API call delay
        
        _logger.LogInformation("Mock: Getting OpenAI service {ServiceName} info", serviceName);

        return new OpenAIServiceInfo
        {
            Id = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.CognitiveServices/accounts/{serviceName}",
            Name = serviceName,
            Endpoint = $"https://{serviceName}.openai.azure.com/",
            Location = "East US",
            Deployments = new List<ModelDeployment>
            {
                new ModelDeployment
                {
                    Name = "gpt-35-turbo",
                    ModelName = "gpt-35-turbo",
                    ModelVersion = "0613",
                    Capacity = 30,
                    Status = "Succeeded"
                },
                new ModelDeployment
                {
                    Name = "gpt-4",
                    ModelName = "gpt-4",
                    ModelVersion = "0613",
                    Capacity = 10,
                    Status = "Succeeded"
                },
                new ModelDeployment
                {
                    Name = "text-embedding-ada-002",
                    ModelName = "text-embedding-ada-002",
                    ModelVersion = "2",
                    Capacity = 30,
                    Status = "Succeeded"
                }
            },
            Usage = new UsageMetrics
            {
                TokensUsed = 125430,
                RequestsPerMinute = 45,
                TokensPerMinute = 2340,
                LastUpdated = DateTime.UtcNow.AddMinutes(-5)
            }
        };
    }

    public async Task<AKSClusterInfo?> GetAKSClusterInfoAsync(string subscriptionId, string resourceGroupName, string clusterName)
    {
        await Task.Delay(600); // Simulate API call delay
        
        _logger.LogInformation("Mock: Getting AKS cluster {ClusterName} info", clusterName);

        return new AKSClusterInfo
        {
            Id = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ContainerService/managedClusters/{clusterName}",
            Name = clusterName,
            Location = "East US",
            KubernetesVersion = "1.28.9",
            Status = "Running",
            NodePools = new List<NodePool>
            {
                new NodePool
                {
                    Name = "system",
                    VmSize = "Standard_D4s_v3",
                    NodeCount = 3,
                    MaxPods = 30,
                    OsType = "Linux",
                    Labels = new Dictionary<string, string> { ["nodepool"] = "system" }
                },
                new NodePool
                {
                    Name = "aiworkload", 
                    VmSize = "Standard_NC4as_T4_v3",
                    NodeCount = 2,
                    MaxPods = 30,
                    OsType = "Linux",
                    Labels = new Dictionary<string, string> 
                    { 
                        ["nodepool"] = "aiworkload",
                        ["accelerator"] = "nvidia-tesla-t4"
                    }
                }
            },
            Metrics = new ClusterMetrics
            {
                CpuUsagePercentage = 68.5,
                MemoryUsagePercentage = 72.1,
                PodCount = 45,
                RunningPods = 42,
                LastUpdated = DateTime.UtcNow.AddMinutes(-2)
            }
        };
    }
}