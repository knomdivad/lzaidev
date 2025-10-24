using LandingZoneAI.Portal.Models;

namespace LandingZoneAI.Portal.Services;

public interface ICostManagementService
{
    Task<List<CostData>> GetResourceCostsAsync(string subscriptionId, string? resourceGroupName = null, TimeSpan? period = null);
    Task<decimal> GetProjectedMonthlyCostAsync(string subscriptionId, string? resourceGroupName = null);
    Task<Dictionary<string, decimal>> GetCostByResourceTypeAsync(string subscriptionId, string? resourceGroupName = null);
    Task<List<CostData>> GetDailyCostTrendAsync(string subscriptionId, string? resourceGroupName = null, int days = 30);
}

public class CostManagementService : ICostManagementService
{
    private readonly ILogger<CostManagementService> _logger;

    public CostManagementService(ILogger<CostManagementService> logger)
    {
        _logger = logger;
    }

    public async Task<List<CostData>> GetResourceCostsAsync(string subscriptionId, string? resourceGroupName = null, TimeSpan? period = null)
    {
        try
        {
            // Implementation would query Azure Cost Management API
            // This would get actual cost data for resources
            
            await Task.Delay(200); // Simulate async operation
            
            var costs = new List<CostData>();
            
            // Simulate some cost data
            var resourceTypes = new[]
            {
                ("Microsoft.MachineLearningServices/workspaces", "ML Workspace", 125.50m),
                ("Microsoft.CognitiveServices/accounts", "OpenAI Service", 89.75m),
                ("Microsoft.ContainerService/managedClusters", "AKS Cluster", 245.30m),
                ("Microsoft.Storage/storageAccounts", "Storage Account", 45.20m),
                ("Microsoft.KeyVault/vaults", "Key Vault", 5.10m),
                ("Microsoft.ContainerRegistry/registries", "Container Registry", 25.80m)
            };

            foreach (var (type, name, dailyCost) in resourceTypes)
            {
                costs.Add(new CostData
                {
                    ResourceId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName ?? "rg-example"}/providers/{type}/example",
                    ResourceName = name,
                    ResourceType = type,
                    DailyCost = dailyCost,
                    MonthlyCost = dailyCost * 30,
                    ProjectedMonthlyCost = dailyCost * 30 * 1.1m, // 10% buffer
                    Currency = "USD",
                    LastUpdated = DateTime.UtcNow
                });
            }

            return costs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resource costs for subscription {SubscriptionId}", subscriptionId);
            throw;
        }
    }

    public async Task<decimal> GetProjectedMonthlyCostAsync(string subscriptionId, string? resourceGroupName = null)
    {
        try
        {
            var costs = await GetResourceCostsAsync(subscriptionId, resourceGroupName);
            return costs.Sum(c => c.ProjectedMonthlyCost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projected monthly cost");
            throw;
        }
    }

    public async Task<Dictionary<string, decimal>> GetCostByResourceTypeAsync(string subscriptionId, string? resourceGroupName = null)
    {
        try
        {
            var costs = await GetResourceCostsAsync(subscriptionId, resourceGroupName);
            
            return costs
                .GroupBy(c => c.ResourceType)
                .ToDictionary(g => g.Key, g => g.Sum(c => c.MonthlyCost));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cost by resource type");
            throw;
        }
    }

    public async Task<List<CostData>> GetDailyCostTrendAsync(string subscriptionId, string? resourceGroupName = null, int days = 30)
    {
        try
        {
            // Implementation would get historical daily cost data
            await Task.Delay(150); // Simulate async operation
            
            var trends = new List<CostData>();
            var random = new Random();
            var baseDate = DateTime.UtcNow.AddDays(-days);
            
            for (int i = 0; i < days; i++)
            {
                trends.Add(new CostData
                {
                    ResourceId = "daily-trend",
                    ResourceName = "Daily Total",
                    ResourceType = "Summary",
                    DailyCost = 450m + (decimal)(random.NextDouble() * 100 - 50), // Vary between 400-500
                    LastUpdated = baseDate.AddDays(i)
                });
            }
            
            return trends;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting daily cost trend");
            throw;
        }
    }
}