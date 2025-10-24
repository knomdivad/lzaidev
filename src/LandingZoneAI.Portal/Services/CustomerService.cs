using LandingZoneAI.Portal.Models;
using LandingZoneAI.Portal.Controllers;

namespace LandingZoneAI.Portal.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetCustomersAsync(CustomerStatus? status = null);
    Task<Customer?> GetCustomerAsync(string customerId);
    Task<Customer> CreateCustomerAsync(CreateCustomerRequest request);
    Task<Customer?> UpdateCustomerAsync(string customerId, UpdateCustomerRequest request);
    Task<CloudProvider> ConfigureCloudProviderAsync(string customerId, CloudProviderConfiguration config);
    Task<SourceControlProvider> ConfigureSourceControlProviderAsync(string customerId, SourceControlConfiguration config);
    Task<List<CustomerLandingZone>> GetCustomerLandingZonesAsync(string customerId);
    Task<List<LandingZoneTemplate>> GetAvailableTemplatesAsync(string customerId);
    Task<Customer?> GetCustomerByEmailAsync(string email);
    Task<bool> DeleteCustomerAsync(string customerId);
}

public class MockCustomerService : ICustomerService
{
    private readonly ILogger<MockCustomerService> _logger;
    private static readonly List<Customer> _customers = new();
    private static readonly List<LandingZoneTemplate> _templates = new();

    static MockCustomerService()
    {
        // Initialize with sample data
        InitializeSampleData();
    }

    public MockCustomerService(ILogger<MockCustomerService> logger)
    {
        _logger = logger;
    }

    public Task<List<Customer>> GetCustomersAsync(CustomerStatus? status = null)
    {
        _logger.LogInformation("Mock: Getting customers with status filter: {Status}", status);
        
        var customers = status.HasValue 
            ? _customers.Where(c => c.Status == status.Value).ToList()
            : _customers.ToList();
            
        return Task.FromResult(customers);
    }

    public Task<Customer?> GetCustomerAsync(string customerId)
    {
        _logger.LogInformation("Mock: Getting customer {CustomerId}", customerId);
        
        var customer = _customers.FirstOrDefault(c => c.Id == customerId);
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetCustomerByEmailAsync(string email)
    {
        _logger.LogInformation("Mock: Getting customer by email {Email}", email);
        
        var customer = _customers.FirstOrDefault(c => c.ContactEmail.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(customer);
    }

    public Task<Customer> CreateCustomerAsync(CreateCustomerRequest request)
    {
        _logger.LogInformation("Mock: Creating customer {Name}", request.Name);
        
        var customer = new Customer
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            ContactEmail = request.ContactEmail,
            CompanyName = request.CompanyName,
            Status = CustomerStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            AvailableTemplates = _templates.Where(t => t.IsActive).ToList()
        };

        _customers.Add(customer);
        return Task.FromResult(customer);
    }

    public Task<Customer?> UpdateCustomerAsync(string customerId, UpdateCustomerRequest request)
    {
        _logger.LogInformation("Mock: Updating customer {CustomerId}", customerId);
        
        var customer = _customers.FirstOrDefault(c => c.Id == customerId);
        if (customer == null)
            return Task.FromResult<Customer?>(null);

        if (!string.IsNullOrEmpty(request.Name))
            customer.Name = request.Name;
        if (!string.IsNullOrEmpty(request.ContactEmail))
            customer.ContactEmail = request.ContactEmail;
        if (!string.IsNullOrEmpty(request.CompanyName))
            customer.CompanyName = request.CompanyName;
        if (request.Status.HasValue)
            customer.Status = request.Status.Value;

        customer.LastUpdated = DateTime.UtcNow;
        
        return Task.FromResult<Customer?>(customer);
    }

    public Task<CloudProvider> ConfigureCloudProviderAsync(string customerId, CloudProviderConfiguration config)
    {
        _logger.LogInformation("Mock: Configuring {CloudProvider} for customer {CustomerId}", config.Type, customerId);
        
        var customer = _customers.FirstOrDefault(c => c.Id == customerId);
        if (customer == null)
            throw new ArgumentException($"Customer {customerId} not found");

        // If setting as default, unset other defaults
        if (config.IsDefault)
        {
            foreach (var existing in customer.CloudProviders)
                existing.IsDefault = false;
        }

        var cloudProvider = new CloudProvider
        {
            Id = Guid.NewGuid().ToString(),
            Type = config.Type,
            DisplayName = config.DisplayName,
            Configuration = config.Configuration,
            IsDefault = config.IsDefault,
            ConfiguredAt = DateTime.UtcNow
        };

        customer.CloudProviders.Add(cloudProvider);
        customer.LastUpdated = DateTime.UtcNow;

        return Task.FromResult(cloudProvider);
    }

    public Task<SourceControlProvider> ConfigureSourceControlProviderAsync(string customerId, SourceControlConfiguration config)
    {
        _logger.LogInformation("Mock: Configuring {SourceControl} for customer {CustomerId}", config.Type, customerId);
        
        var customer = _customers.FirstOrDefault(c => c.Id == customerId);
        if (customer == null)
            throw new ArgumentException($"Customer {customerId} not found");

        // If setting as default, unset other defaults
        if (config.IsDefault)
        {
            foreach (var existing in customer.SourceControlProviders)
                existing.IsDefault = false;
        }

        var sourceControlProvider = new SourceControlProvider
        {
            Id = Guid.NewGuid().ToString(),
            Type = config.Type,
            DisplayName = config.DisplayName,
            RepositoryUrl = config.RepositoryUrl,
            Organization = config.Organization,
            Configuration = config.Configuration,
            IsDefault = config.IsDefault,
            ConfiguredAt = DateTime.UtcNow
        };

        customer.SourceControlProviders.Add(sourceControlProvider);
        customer.LastUpdated = DateTime.UtcNow;

        return Task.FromResult(sourceControlProvider);
    }

    public Task<List<CustomerLandingZone>> GetCustomerLandingZonesAsync(string customerId)
    {
        _logger.LogInformation("Mock: Getting landing zones for customer {CustomerId}", customerId);
        
        var customer = _customers.FirstOrDefault(c => c.Id == customerId);
        if (customer == null)
            return Task.FromResult(new List<CustomerLandingZone>());

        return Task.FromResult(customer.LandingZones);
    }

    public Task<List<LandingZoneTemplate>> GetAvailableTemplatesAsync(string customerId)
    {
        _logger.LogInformation("Mock: Getting available templates for customer {CustomerId}", customerId);
        
        return Task.FromResult(_templates.Where(t => t.IsActive).ToList());
    }

    public Task<bool> DeleteCustomerAsync(string customerId)
    {
        _logger.LogInformation("Mock: Deleting customer {CustomerId}", customerId);
        
        var customer = _customers.FirstOrDefault(c => c.Id == customerId);
        if (customer == null)
            return Task.FromResult(false);

        _customers.Remove(customer);
        return Task.FromResult(true);
    }

    private static void InitializeSampleData()
    {
        // Initialize templates
        _templates.AddRange(new[]
        {
            new LandingZoneTemplate
            {
                Id = "template-basic-ai",
                Name = "Basic AI Development",
                Description = "Simple AI development environment with ML workspace and basic compute",
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
                        Description = "Deployment environment",
                        Type = ParameterType.String,
                        DefaultValue = "dev",
                        Required = true,
                        AllowedValues = new List<string> { "dev", "staging", "prod" }
                    },
                    ["nodeCount"] = new TemplateParameter
                    {
                        Name = "nodeCount",
                        DisplayName = "Node Count",
                        Description = "Number of compute nodes",
                        Type = ParameterType.Number,
                        DefaultValue = 2,
                        Required = true
                    }
                },
                RequiredFeatures = new List<string> { "Machine Learning", "Basic Storage" }
            },
            new LandingZoneTemplate
            {
                Id = "template-enterprise-ai",
                Name = "Enterprise AI Platform",
                Description = "Full enterprise AI platform with security, compliance, and governance",
                Version = "1.2.0",
                Type = TemplateType.EnterpriseAI,
                SupportedCloudProviders = new List<CloudProviderType> { CloudProviderType.Azure },
                EstimatedMonthlyCost = 2500m,
                Parameters = new Dictionary<string, TemplateParameter>
                {
                    ["enablePrivateEndpoints"] = new TemplateParameter
                    {
                        Name = "enablePrivateEndpoints",
                        DisplayName = "Private Endpoints",
                        Description = "Enable private network endpoints",
                        Type = ParameterType.Boolean,
                        DefaultValue = true,
                        Required = false
                    },
                    ["nodeCount"] = new TemplateParameter
                    {
                        Name = "nodeCount",
                        DisplayName = "Node Count",
                        Description = "Number of compute nodes",
                        Type = ParameterType.Number,
                        DefaultValue = 5,
                        Required = true
                    }
                },
                RequiredFeatures = new List<string> { "Machine Learning", "Data Lake", "Security", "Compliance" }
            },
            new LandingZoneTemplate
            {
                Id = "template-mlops",
                Name = "MLOps Pipeline",
                Description = "Complete MLOps pipeline with CI/CD for machine learning models",
                Version = "1.1.0",
                Type = TemplateType.MLOps,
                SupportedCloudProviders = new List<CloudProviderType> { CloudProviderType.Azure, CloudProviderType.AWS },
                EstimatedMonthlyCost = 1200m,
                Parameters = new Dictionary<string, TemplateParameter>
                {
                    ["pipelineType"] = new TemplateParameter
                    {
                        Name = "pipelineType",
                        DisplayName = "Pipeline Type",
                        Description = "Type of MLOps pipeline",
                        Type = ParameterType.String,
                        DefaultValue = "standard",
                        Required = true,
                        AllowedValues = new List<string> { "basic", "standard", "advanced" }
                    }
                },
                RequiredFeatures = new List<string> { "Machine Learning", "CI/CD", "Model Registry" }
            }
        });

        // Initialize sample customers
        var sampleCustomer = new Customer
        {
            Id = "customer-sample-1",
            Name = "John Smith",
            ContactEmail = "john.smith@example.com",
            CompanyName = "TechCorp AI Division",
            Status = CustomerStatus.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            LastUpdated = DateTime.UtcNow.AddDays(-2),
            CloudProviders = new List<CloudProvider>
            {
                new CloudProvider
                {
                    Id = "cp-azure-1",
                    Type = CloudProviderType.Azure,
                    DisplayName = "Production Azure",
                    IsDefault = true,
                    Configuration = new Dictionary<string, string>
                    {
                        ["subscriptionId"] = "12345678-1234-1234-1234-123456789012",
                        ["tenantId"] = "87654321-4321-4321-4321-210987654321"
                    }
                }
            },
            SourceControlProviders = new List<SourceControlProvider>
            {
                new SourceControlProvider
                {
                    Id = "scp-github-1",
                    Type = SourceControlType.GitHub,
                    DisplayName = "Company GitHub",
                    Organization = "techcorp-ai",
                    IsDefault = true,
                    Configuration = new Dictionary<string, string>
                    {
                        ["token"] = "ghp_xxxxxxxxxxxxxxxxxxxx",
                        ["defaultBranch"] = "main"
                    }
                }
            },
            LandingZones = new List<CustomerLandingZone>
            {
                new CustomerLandingZone
                {
                    Id = "lz-dev-001",
                    CustomerId = "customer-sample-1",
                    Name = "Development Environment",
                    Environment = "dev",
                    TemplateId = "template-basic-ai",
                    CloudProviderId = "cp-azure-1",
                    Status = LandingZoneStatus.Deployed,
                    Parameters = new Dictionary<string, object>
                    {
                        ["environment"] = "dev",
                        ["nodeCount"] = 3
                    },
                    ResourceGroupName = "rg-techcorp-ai-dev",
                    SubscriptionId = "12345678-1234-1234-1234-123456789012",
                    CurrentMonthlyCost = 450m,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    DeployedAt = DateTime.UtcNow.AddDays(-14)
                }
            },
            AvailableTemplates = _templates.ToList()
        };

        _customers.Add(sampleCustomer);
    }
}