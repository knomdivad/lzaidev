namespace LandingZoneAI.Portal.Models;

public class ResourceSummary
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ResourceGroup { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Dictionary<string, string> Tags { get; set; } = new();
}

public class MLWorkspaceInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string DiscoveryUrl { get; set; } = string.Empty;
    public string WorkspaceUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<ComputeInstance> ComputeInstances { get; set; } = new();
    public List<MLModel> Models { get; set; } = new();
}

public class ComputeInstance
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string VmSize { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; }
}

public class MLModel
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Framework { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class OpenAIServiceInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<ModelDeployment> Deployments { get; set; } = new();
    public UsageMetrics Usage { get; set; } = new();
}

public class ModelDeployment
{
    public string Name { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public string ModelVersion { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UsageMetrics
{
    public int TokensUsed { get; set; }
    public int RequestsPerMinute { get; set; }
    public int TokensPerMinute { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class AKSClusterInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string KubernetesVersion { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<NodePool> NodePools { get; set; } = new();
    public ClusterMetrics Metrics { get; set; } = new();
}

public class NodePool
{
    public string Name { get; set; } = string.Empty;
    public string VmSize { get; set; } = string.Empty;
    public int NodeCount { get; set; }
    public int MaxPods { get; set; }
    public string OsType { get; set; } = string.Empty;
    public Dictionary<string, string> Labels { get; set; } = new();
}

public class ClusterMetrics
{
    public double CpuUsagePercentage { get; set; }
    public double MemoryUsagePercentage { get; set; }
    public int PodCount { get; set; }
    public int RunningPods { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class CostData
{
    public string ResourceId { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public decimal DailyCost { get; set; }
    public decimal MonthlyCost { get; set; }
    public decimal ProjectedMonthlyCost { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime LastUpdated { get; set; }
}

public class DeploymentRequest
{
    public string Environment { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class DeploymentStatus
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ResourceGroup { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public List<string> Operations { get; set; } = new();
    public string? ErrorMessage { get; set; }
}