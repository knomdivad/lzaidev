using LandingZoneAI.Portal.Models;
using System.Text.Json;

namespace LandingZoneAI.Portal.Services;

public interface ITemplateManagementService
{
    Task<List<LandingZoneTemplate>> GetTemplatesAsync();
    Task<LandingZoneTemplate?> GetTemplateAsync(string templateId);
    Task<LandingZoneTemplate> CreateTemplateAsync(CreateTemplateRequest request);
    Task<LandingZoneTemplate?> UpdateTemplateAsync(string templateId, UpdateTemplateRequest request);
    Task<bool> DeleteTemplateAsync(string templateId);
    Task<List<LandingZoneTemplate>> GetTemplatesByCategoryAsync(TemplateType category);
    Task<List<LandingZoneTemplate>> GetTemplatesByCloudProviderAsync(CloudProviderType cloudProvider);
    Task<LandingZoneTemplate?> CloneTemplateAsync(string templateId, string newName);
    Task<bool> ValidateTemplateAsync(LandingZoneTemplate template);
    Task<Dictionary<string, object>> GetTemplateDefaultParametersAsync(string templateId);
}

public class MockTemplateManagementService : ITemplateManagementService
{
    private readonly ILogger<MockTemplateManagementService> _logger;
    private static readonly List<LandingZoneTemplate> _templates = new();

    static MockTemplateManagementService()
    {
        InitializeTemplates();
    }

    public MockTemplateManagementService(ILogger<MockTemplateManagementService> logger)
    {
        _logger = logger;
    }

    public Task<List<LandingZoneTemplate>> GetTemplatesAsync()
    {
        _logger.LogInformation("Mock Template: Getting all templates");
        return Task.FromResult(_templates.ToList());
    }

    public Task<LandingZoneTemplate?> GetTemplateAsync(string templateId)
    {
        _logger.LogInformation("Mock Template: Getting template {TemplateId}", templateId);
        
        var template = _templates.FirstOrDefault(t => t.Id == templateId);
        return Task.FromResult(template);
    }

    public Task<LandingZoneTemplate> CreateTemplateAsync(CreateTemplateRequest request)
    {
        _logger.LogInformation("Mock Template: Creating template {Name}", request.Name);

        var template = new LandingZoneTemplate
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            Version = request.Version ?? "1.0.0",
            Type = request.Type,
            SupportedCloudProviders = request.SupportedCloudProviders?.ToList() ?? new List<CloudProviderType>(),
            Parameters = request.Parameters ?? new Dictionary<string, TemplateParameter>(),
            RequiredFeatures = request.RequiredFeatures?.ToList() ?? new List<string>(),
            EstimatedMonthlyCost = request.EstimatedMonthlyCost,
            IsActive = true
        };

        _templates.Add(template);
        return Task.FromResult(template);
    }

    public Task<LandingZoneTemplate?> UpdateTemplateAsync(string templateId, UpdateTemplateRequest request)
    {
        _logger.LogInformation("Mock Template: Updating template {TemplateId}", templateId);

        var template = _templates.FirstOrDefault(t => t.Id == templateId);
        if (template == null)
            return Task.FromResult<LandingZoneTemplate?>(null);

        if (!string.IsNullOrEmpty(request.Name))
            template.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Description))
            template.Description = request.Description;
        if (!string.IsNullOrEmpty(request.Version))
            template.Version = request.Version;
        if (request.Type.HasValue)
            template.Type = request.Type.Value;
        if (request.SupportedCloudProviders?.Any() == true)
            template.SupportedCloudProviders = request.SupportedCloudProviders.ToList();
        if (request.Parameters?.Any() == true)
            template.Parameters = request.Parameters;
        if (request.RequiredFeatures?.Any() == true)
            template.RequiredFeatures = request.RequiredFeatures.ToList();
        if (request.EstimatedMonthlyCost.HasValue)
            template.EstimatedMonthlyCost = request.EstimatedMonthlyCost;
        if (request.IsActive.HasValue)
            template.IsActive = request.IsActive.Value;

        return Task.FromResult<LandingZoneTemplate?>(template);
    }

    public Task<bool> DeleteTemplateAsync(string templateId)
    {
        _logger.LogInformation("Mock Template: Deleting template {TemplateId}", templateId);

        var template = _templates.FirstOrDefault(t => t.Id == templateId);
        if (template == null)
            return Task.FromResult(false);

        _templates.Remove(template);
        return Task.FromResult(true);
    }

    public Task<List<LandingZoneTemplate>> GetTemplatesByCategoryAsync(TemplateType category)
    {
        _logger.LogInformation("Mock Template: Getting templates by category {Category}", category);

        var templates = _templates.Where(t => t.Type == category && t.IsActive).ToList();
        return Task.FromResult(templates);
    }

    public Task<List<LandingZoneTemplate>> GetTemplatesByCloudProviderAsync(CloudProviderType cloudProvider)
    {
        _logger.LogInformation("Mock Template: Getting templates by cloud provider {CloudProvider}", cloudProvider);

        var templates = _templates.Where(t => t.SupportedCloudProviders.Contains(cloudProvider) && t.IsActive).ToList();
        return Task.FromResult(templates);
    }

    public Task<LandingZoneTemplate?> CloneTemplateAsync(string templateId, string newName)
    {
        _logger.LogInformation("Mock Template: Cloning template {TemplateId} as {NewName}", templateId, newName);

        var originalTemplate = _templates.FirstOrDefault(t => t.Id == templateId);
        if (originalTemplate == null)
            return Task.FromResult<LandingZoneTemplate?>(null);

        var clonedTemplate = new LandingZoneTemplate
        {
            Id = Guid.NewGuid().ToString(),
            Name = newName,
            Description = $"Cloned from: {originalTemplate.Description}",
            Version = "1.0.0", // Reset version for clone
            Type = originalTemplate.Type,
            SupportedCloudProviders = new List<CloudProviderType>(originalTemplate.SupportedCloudProviders),
            Parameters = JsonSerializer.Deserialize<Dictionary<string, TemplateParameter>>(JsonSerializer.Serialize(originalTemplate.Parameters)) ?? new Dictionary<string, TemplateParameter>(),
            RequiredFeatures = new List<string>(originalTemplate.RequiredFeatures),
            EstimatedMonthlyCost = originalTemplate.EstimatedMonthlyCost,
            IsActive = true
        };

        _templates.Add(clonedTemplate);
        return Task.FromResult<LandingZoneTemplate?>(clonedTemplate);
    }

    public Task<bool> ValidateTemplateAsync(LandingZoneTemplate template)
    {
        _logger.LogInformation("Mock Template: Validating template {TemplateId}", template.Id);

        var isValid = true;
        var errors = new List<string>();

        // Basic validation
        if (string.IsNullOrEmpty(template.Name))
        {
            errors.Add("Template name is required");
            isValid = false;
        }

        if (string.IsNullOrEmpty(template.Description))
        {
            errors.Add("Template description is required");
            isValid = false;
        }

        if (!template.SupportedCloudProviders.Any())
        {
            errors.Add("At least one cloud provider must be supported");
            isValid = false;
        }

        // Validate parameters
        foreach (var param in template.Parameters)
        {
            if (string.IsNullOrEmpty(param.Value.Name))
            {
                errors.Add($"Parameter {param.Key} must have a name");
                isValid = false;
            }

            if (param.Value.Required && param.Value.DefaultValue == null)
            {
                errors.Add($"Required parameter {param.Key} must have a default value");
                isValid = false;
            }
        }

        if (!isValid)
        {
            _logger.LogWarning("Template validation failed: {Errors}", string.Join(", ", errors));
        }

        return Task.FromResult(isValid);
    }

    public Task<Dictionary<string, object>> GetTemplateDefaultParametersAsync(string templateId)
    {
        _logger.LogInformation("Mock Template: Getting default parameters for template {TemplateId}", templateId);

        var template = _templates.FirstOrDefault(t => t.Id == templateId);
        if (template == null)
            return Task.FromResult(new Dictionary<string, object>());

        var defaultParameters = new Dictionary<string, object>();
        
        foreach (var param in template.Parameters)
        {
            if (param.Value.DefaultValue != null)
            {
                defaultParameters[param.Key] = param.Value.DefaultValue;
            }
        }

        return Task.FromResult(defaultParameters);
    }

    private static void InitializeTemplates()
    {
        _templates.AddRange(new[]
        {
            new LandingZoneTemplate
            {
                Id = "template-basic-ai",
                Name = "Basic AI Development",
                Description = "Essential AI development environment with Azure ML Workspace, basic compute, and storage for small teams and proof-of-concept projects.",
                Version = "1.0.0",
                Type = TemplateType.BasicAI,
                SupportedCloudProviders = new List<CloudProviderType> { CloudProviderType.Azure, CloudProviderType.AWS },
                EstimatedMonthlyCost = 500m,
                Parameters = new Dictionary<string, TemplateParameter>
                {
                    ["environment"] = new TemplateParameter
                    {
                        Name = "environment",
                        DisplayName = "Environment",
                        Description = "Deployment environment (dev, staging, prod)",
                        Type = ParameterType.String,
                        DefaultValue = "dev",
                        Required = true,
                        AllowedValues = new List<string> { "dev", "staging", "prod" }
                    },
                    ["nodeCount"] = new TemplateParameter
                    {
                        Name = "nodeCount",
                        DisplayName = "Compute Node Count",
                        Description = "Number of compute nodes for the cluster",
                        Type = ParameterType.Number,
                        DefaultValue = 2,
                        Required = true,
                        ValidationRules = new Dictionary<string, object> { ["min"] = 1, ["max"] = 5 }
                    },
                    ["vmSize"] = new TemplateParameter
                    {
                        Name = "vmSize",
                        DisplayName = "VM Size",
                        Description = "Virtual machine size for compute nodes",
                        Type = ParameterType.String,
                        DefaultValue = "Standard_D4s_v3",
                        Required = true,
                        AllowedValues = new List<string> { "Standard_D2s_v3", "Standard_D4s_v3", "Standard_D8s_v3" }
                    },
                    ["enablePrivateEndpoints"] = new TemplateParameter
                    {
                        Name = "enablePrivateEndpoints",
                        DisplayName = "Private Network Endpoints",
                        Description = "Enable private endpoints for enhanced security",
                        Type = ParameterType.Boolean,
                        DefaultValue = false,
                        Required = false
                    }
                },
                RequiredFeatures = new List<string> { "Machine Learning", "Basic Storage", "Container Registry" },
                IsActive = true
            },
            new LandingZoneTemplate
            {
                Id = "template-enterprise-ai",
                Name = "Enterprise AI Platform",
                Description = "Comprehensive enterprise-grade AI platform with advanced security, compliance features, governance, and high-availability for production workloads.",
                Version = "1.2.0",
                Type = TemplateType.EnterpriseAI,
                SupportedCloudProviders = new List<CloudProviderType> { CloudProviderType.Azure },
                EstimatedMonthlyCost = 2500m,
                Parameters = new Dictionary<string, TemplateParameter>
                {
                    ["environment"] = new TemplateParameter
                    {
                        Name = "environment",
                        DisplayName = "Environment",
                        Description = "Deployment environment",
                        Type = ParameterType.String,
                        DefaultValue = "prod",
                        Required = true,
                        AllowedValues = new List<string> { "staging", "prod" }
                    },
                    ["nodeCount"] = new TemplateParameter
                    {
                        Name = "nodeCount",
                        DisplayName = "System Node Count",
                        Description = "Number of system nodes for the cluster",
                        Type = ParameterType.Number,
                        DefaultValue = 5,
                        Required = true,
                        ValidationRules = new Dictionary<string, object> { ["min"] = 3, ["max"] = 20 }
                    },
                    ["aiNodeCount"] = new TemplateParameter
                    {
                        Name = "aiNodeCount",
                        DisplayName = "AI Workload Node Count",
                        Description = "Number of dedicated AI/GPU nodes",
                        Type = ParameterType.Number,
                        DefaultValue = 3,
                        Required = true,
                        ValidationRules = new Dictionary<string, object> { ["min"] = 1, ["max"] = 10 }
                    },
                    ["enablePrivateEndpoints"] = new TemplateParameter
                    {
                        Name = "enablePrivateEndpoints",
                        DisplayName = "Private Network Endpoints",
                        Description = "Enable private endpoints for all services",
                        Type = ParameterType.Boolean,
                        DefaultValue = true,
                        Required = false
                    },
                    ["enableBackup"] = new TemplateParameter
                    {
                        Name = "enableBackup",
                        DisplayName = "Automated Backup",
                        Description = "Enable automated backup for data and models",
                        Type = ParameterType.Boolean,
                        DefaultValue = true,
                        Required = false
                    },
                    ["complianceStandard"] = new TemplateParameter
                    {
                        Name = "complianceStandard",
                        DisplayName = "Compliance Standard",
                        Description = "Required compliance standard",
                        Type = ParameterType.String,
                        DefaultValue = "SOC2",
                        Required = false,
                        AllowedValues = new List<string> { "SOC2", "HIPAA", "PCI-DSS", "ISO27001" }
                    }
                },
                RequiredFeatures = new List<string> { "Machine Learning", "Data Lake", "Security", "Compliance", "High Availability", "GPU Compute" },
                IsActive = true
            },
            new LandingZoneTemplate
            {
                Id = "template-mlops",
                Name = "MLOps Pipeline",
                Description = "Complete MLOps platform with CI/CD pipelines, model registry, automated testing, and deployment orchestration for machine learning lifecycle management.",
                Version = "1.1.0",
                Type = TemplateType.MLOps,
                SupportedCloudProviders = new List<CloudProviderType> { CloudProviderType.Azure, CloudProviderType.AWS },
                EstimatedMonthlyCost = 1200m,
                Parameters = new Dictionary<string, TemplateParameter>
                {
                    ["pipelineType"] = new TemplateParameter
                    {
                        Name = "pipelineType",
                        DisplayName = "Pipeline Complexity",
                        Description = "Type of MLOps pipeline configuration",
                        Type = ParameterType.String,
                        DefaultValue = "standard",
                        Required = true,
                        AllowedValues = new List<string> { "basic", "standard", "advanced" }
                    },
                    ["nodeCount"] = new TemplateParameter
                    {
                        Name = "nodeCount",
                        DisplayName = "Pipeline Node Count",
                        Description = "Number of nodes for pipeline execution",
                        Type = ParameterType.Number,
                        DefaultValue = 3,
                        Required = true,
                        ValidationRules = new Dictionary<string, object> { ["min"] = 2, ["max"] = 8 }
                    },
                    ["enableModelMonitoring"] = new TemplateParameter
                    {
                        Name = "enableModelMonitoring",
                        DisplayName = "Model Monitoring",
                        Description = "Enable automated model performance monitoring",
                        Type = ParameterType.Boolean,
                        DefaultValue = true,
                        Required = false
                    },
                    ["deploymentStrategy"] = new TemplateParameter
                    {
                        Name = "deploymentStrategy",
                        DisplayName = "Deployment Strategy",
                        Description = "Model deployment strategy",
                        Type = ParameterType.String,
                        DefaultValue = "blue-green",
                        Required = false,
                        AllowedValues = new List<string> { "rolling", "blue-green", "canary" }
                    }
                },
                RequiredFeatures = new List<string> { "Machine Learning", "CI/CD", "Model Registry", "Pipeline Orchestration", "Model Monitoring" },
                IsActive = true
            },
            new LandingZoneTemplate
            {
                Id = "template-data-science",
                Name = "Data Science Workspace",
                Description = "Collaborative data science environment with Jupyter notebooks, shared datasets, experiment tracking, and team collaboration tools.",
                Version = "1.0.0",
                Type = TemplateType.DataScience,
                SupportedCloudProviders = new List<CloudProviderType> { CloudProviderType.Azure, CloudProviderType.AWS, CloudProviderType.GCP },
                EstimatedMonthlyCost = 800m,
                Parameters = new Dictionary<string, TemplateParameter>
                {
                    ["teamSize"] = new TemplateParameter
                    {
                        Name = "teamSize",
                        DisplayName = "Team Size",
                        Description = "Number of data scientists using the workspace",
                        Type = ParameterType.Number,
                        DefaultValue = 5,
                        Required = true,
                        ValidationRules = new Dictionary<string, object> { ["min"] = 1, ["max"] = 20 }
                    },
                    ["storageSize"] = new TemplateParameter
                    {
                        Name = "storageSize",
                        DisplayName = "Storage Size (GB)",
                        Description = "Amount of storage for datasets and notebooks",
                        Type = ParameterType.Number,
                        DefaultValue = 1000,
                        Required = true,
                        ValidationRules = new Dictionary<string, object> { ["min"] = 100, ["max"] = 10000 }
                    },
                    ["enableGPU"] = new TemplateParameter
                    {
                        Name = "enableGPU",
                        DisplayName = "GPU Support",
                        Description = "Enable GPU instances for deep learning",
                        Type = ParameterType.Boolean,
                        DefaultValue = false,
                        Required = false
                    }
                },
                RequiredFeatures = new List<string> { "Jupyter Notebooks", "Shared Storage", "Experiment Tracking", "Team Collaboration" },
                IsActive = true
            },
            new LandingZoneTemplate
            {
                Id = "template-custom-ai",
                Name = "Custom AI Solution",
                Description = "Flexible template for custom AI solutions with configurable components based on specific requirements and use cases.",
                Version = "1.0.0",
                Type = TemplateType.CustomAI,
                SupportedCloudProviders = new List<CloudProviderType> { CloudProviderType.Azure, CloudProviderType.AWS, CloudProviderType.GCP },
                EstimatedMonthlyCost = null, // Will be calculated based on selected components
                Parameters = new Dictionary<string, TemplateParameter>
                {
                    ["aiServices"] = new TemplateParameter
                    {
                        Name = "aiServices",
                        DisplayName = "AI Services",
                        Description = "Select required AI services",
                        Type = ParameterType.List,
                        DefaultValue = new List<string> { "Machine Learning" },
                        Required = true,
                        AllowedValues = new List<string> { "Machine Learning", "Computer Vision", "Natural Language Processing", "Speech Services", "OpenAI" }
                    },
                    ["computeType"] = new TemplateParameter
                    {
                        Name = "computeType",
                        DisplayName = "Compute Type",
                        Description = "Primary compute type for workloads",
                        Type = ParameterType.String,
                        DefaultValue = "cpu",
                        Required = true,
                        AllowedValues = new List<string> { "cpu", "gpu", "mixed" }
                    },
                    ["scalingModel"] = new TemplateParameter
                    {
                        Name = "scalingModel",
                        DisplayName = "Scaling Model",
                        Description = "How the infrastructure should scale",
                        Type = ParameterType.String,
                        DefaultValue = "auto",
                        Required = false,
                        AllowedValues = new List<string> { "fixed", "auto", "scheduled" }
                    }
                },
                RequiredFeatures = new List<string> { "Configurable Components", "Flexible Scaling" },
                IsActive = true
            }
        });
    }
}

/// <summary>
/// Request models for template management
/// </summary>
public class CreateTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Version { get; set; }
    public TemplateType Type { get; set; }
    public IEnumerable<CloudProviderType>? SupportedCloudProviders { get; set; }
    public Dictionary<string, TemplateParameter>? Parameters { get; set; }
    public IEnumerable<string>? RequiredFeatures { get; set; }
    public decimal? EstimatedMonthlyCost { get; set; }
}

public class UpdateTemplateRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Version { get; set; }
    public TemplateType? Type { get; set; }
    public IEnumerable<CloudProviderType>? SupportedCloudProviders { get; set; }
    public Dictionary<string, TemplateParameter>? Parameters { get; set; }
    public IEnumerable<string>? RequiredFeatures { get; set; }
    public decimal? EstimatedMonthlyCost { get; set; }
    public bool? IsActive { get; set; }
}