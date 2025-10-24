using System.ComponentModel.DataAnnotations;

namespace LandingZoneAI.Portal.Models;

/// <summary>
/// Customer/Tenant configuration model
/// </summary>
public class Customer
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;
    
    public string? CompanyName { get; set; }
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    // Cloud Configuration
    public List<CloudProvider> CloudProviders { get; set; } = new();
    
    // Source Control Configuration  
    public List<SourceControlProvider> SourceControlProviders { get; set; } = new();
    
    // Landing Zone Templates
    public List<LandingZoneTemplate> AvailableTemplates { get; set; } = new();
    
    // Customer's deployed landing zones
    public List<CustomerLandingZone> LandingZones { get; set; } = new();
}

public enum CustomerStatus
{
    Active,
    Inactive,
    Suspended,
    Trial
}

/// <summary>
/// Cloud provider configuration for customer
/// </summary>
public class CloudProvider
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public CloudProviderType Type { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public Dictionary<string, string> Configuration { get; set; } = new();
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ConfiguredAt { get; set; } = DateTime.UtcNow;
}

public enum CloudProviderType
{
    Azure,
    AWS,
    GCP,
    OnPremises
}

/// <summary>
/// Source control provider configuration
/// </summary>
public class SourceControlProvider
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public SourceControlType Type { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? RepositoryUrl { get; set; }
    public string? Organization { get; set; }
    public Dictionary<string, string> Configuration { get; set; } = new();
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ConfiguredAt { get; set; } = DateTime.UtcNow;
}

public enum SourceControlType
{
    GitHub,
    GitLab,
    AzureDevOps,
    BitBucket
}

/// <summary>
/// Landing zone template configuration
/// </summary>
public class LandingZoneTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public TemplateType Type { get; set; }
    public List<CloudProviderType> SupportedCloudProviders { get; set; } = new();
    public Dictionary<string, TemplateParameter> Parameters { get; set; } = new();
    public List<string> RequiredFeatures { get; set; } = new();
    public decimal? EstimatedMonthlyCost { get; set; }
    public bool IsActive { get; set; } = true;
}

public enum TemplateType
{
    BasicAI,
    EnterpriseAI,
    MLOps,
    DataScience,
    CustomAI
}

public class TemplateParameter
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ParameterType Type { get; set; }
    public object? DefaultValue { get; set; }
    public bool Required { get; set; }
    public List<string>? AllowedValues { get; set; }
    public Dictionary<string, object>? ValidationRules { get; set; }
}

public enum ParameterType
{
    String,
    Number,
    Boolean,
    List,
    Object
}

/// <summary>
/// Customer's deployed landing zone instance
/// </summary>
public class CustomerLandingZone
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CustomerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty; // dev, staging, prod
    public string TemplateId { get; set; } = string.Empty;
    public string CloudProviderId { get; set; } = string.Empty;
    public string? SourceControlProviderId { get; set; }
    public LandingZoneStatus Status { get; set; } = LandingZoneStatus.Requested;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string? ResourceGroupName { get; set; }
    public string? SubscriptionId { get; set; }
    public decimal? CurrentMonthlyCost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeployedAt { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public List<DeploymentHistory> DeploymentHistory { get; set; } = new();
}

public enum LandingZoneStatus
{
    Requested,
    Provisioning,
    Deployed,
    Failed,
    Updating,
    Destroying,
    Destroyed
}

public class DeploymentHistory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Action { get; set; } = string.Empty; // deploy, update, destroy
    public LandingZoneStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object>? Changes { get; set; }
}

/// <summary>
/// AI Chat interaction for landing zone creation
/// </summary>
public class AIConversation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CustomerId { get; set; } = string.Empty;
    public string? LandingZoneId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    public ConversationStatus Status { get; set; } = ConversationStatus.Active;
    public List<AIMessage> Messages { get; set; } = new();
    public Dictionary<string, object> Context { get; set; } = new();
    public LandingZoneRequirements? CollectedRequirements { get; set; }
}

public enum ConversationStatus
{
    Active,
    Completed,
    Abandoned,
    Error
}

public class AIMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public MessageType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? Metadata { get; set; }
}

public enum MessageType
{
    UserMessage,
    AIResponse,
    SystemMessage,
    RequirementCollected,
    TemplateRecommendation,
    DeploymentProgress
}

/// <summary>
/// Requirements collected through AI interaction
/// </summary>
public class LandingZoneRequirements
{
    public string? ProjectName { get; set; }
    public string? Environment { get; set; }
    public CloudProviderType? PreferredCloudProvider { get; set; }
    public List<string> RequiredServices { get; set; } = new();
    public List<string> MLFrameworks { get; set; } = new();
    public ComputeRequirements? ComputeRequirements { get; set; }
    public SecurityRequirements? SecurityRequirements { get; set; }
    public NetworkingRequirements? NetworkingRequirements { get; set; }
    public CostConstraints? CostConstraints { get; set; }
    public string? Region { get; set; }
    public bool? RequiresGPU { get; set; }
    public DataRequirements? DataRequirements { get; set; }
}

public class ComputeRequirements
{
    public int? MinNodes { get; set; }
    public int? MaxNodes { get; set; }
    public string? PreferredVMSize { get; set; }
    public bool? AutoScalingEnabled { get; set; }
    public List<string> Workloads { get; set; } = new();
}

public class SecurityRequirements
{
    public bool? RequirePrivateEndpoints { get; set; }
    public bool? RequireNetworkIsolation { get; set; }
    public List<string> ComplianceStandards { get; set; } = new();
    public bool? RequireKeyVault { get; set; }
}

public class NetworkingRequirements
{
    public string? PreferredAddressSpace { get; set; }
    public bool? RequireVPN { get; set; }
    public bool? RequireExpressRoute { get; set; }
    public List<string> AllowedSourceIPs { get; set; } = new();
}

public class CostConstraints
{
    public decimal? MaxMonthlyCost { get; set; }
    public decimal? MaxDailyCost { get; set; }
    public bool? RequireCostAlerts { get; set; }
    public string? BillingAccount { get; set; }
}

public class DataRequirements
{
    public bool? RequiresDataLake { get; set; }
    public List<string> DataSources { get; set; } = new();
    public string? DataSensitivityLevel { get; set; }
    public bool? RequiresBackup { get; set; }
    public int? RetentionDays { get; set; }
}