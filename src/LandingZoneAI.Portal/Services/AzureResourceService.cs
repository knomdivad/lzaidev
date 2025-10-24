using LandingZoneAI.Portal.Models;

namespace LandingZoneAI.Portal.Services;

public interface IAzureResourceService
{
    Task<List<ResourceSummary>> GetResourcesAsync(string subscriptionId, string? resourceGroupName = null);
    Task<List<string>> GetResourceGroupsAsync(string subscriptionId);
    Task<MLWorkspaceInfo?> GetMLWorkspaceInfoAsync(string subscriptionId, string resourceGroupName, string workspaceName);
    Task<OpenAIServiceInfo?> GetOpenAIServiceInfoAsync(string subscriptionId, string resourceGroupName, string serviceName);
    Task<AKSClusterInfo?> GetAKSClusterInfoAsync(string subscriptionId, string resourceGroupName, string clusterName);
}

/// <summary>
/// Azure Resource Service implementation - requires Azure SDK packages to function
/// Use MockAzureResourceService for local development without Azure dependencies
/// </summary>
public class AzureResourceService : IAzureResourceService
{
    private readonly ILogger<AzureResourceService> _logger;

    public AzureResourceService(ILogger<AzureResourceService> logger)
    {
        _logger = logger;
    }

    public async Task<List<ResourceSummary>> GetResourcesAsync(string subscriptionId, string? resourceGroupName = null)
    {
        await Task.Delay(100);
        _logger.LogError("AzureResourceService requires Azure SDK packages. Use MockAzureResourceService for local development.");
        throw new NotImplementedException("Azure SDK packages not available. Use MockAzureResourceService for local development.");
    }

    public async Task<List<string>> GetResourceGroupsAsync(string subscriptionId)
    {
        await Task.Delay(100);
        _logger.LogError("AzureResourceService requires Azure SDK packages. Use MockAzureResourceService for local development.");
        throw new NotImplementedException("Azure SDK packages not available. Use MockAzureResourceService for local development.");
    }

    public async Task<MLWorkspaceInfo?> GetMLWorkspaceInfoAsync(string subscriptionId, string resourceGroupName, string workspaceName)
    {
        await Task.Delay(100);
        _logger.LogError("AzureResourceService requires Azure SDK packages. Use MockAzureResourceService for local development.");
        throw new NotImplementedException("Azure SDK packages not available. Use MockAzureResourceService for local development.");
    }

    public async Task<OpenAIServiceInfo?> GetOpenAIServiceInfoAsync(string subscriptionId, string resourceGroupName, string serviceName)
    {
        await Task.Delay(100);
        _logger.LogError("AzureResourceService requires Azure SDK packages. Use MockAzureResourceService for local development.");
        throw new NotImplementedException("Azure SDK packages not available. Use MockAzureResourceService for local development.");
    }

    public async Task<AKSClusterInfo?> GetAKSClusterInfoAsync(string subscriptionId, string resourceGroupName, string clusterName)
    {
        await Task.Delay(100);
        _logger.LogError("AzureResourceService requires Azure SDK packages. Use MockAzureResourceService for local development.");
        throw new NotImplementedException("Azure SDK packages not available. Use MockAzureResourceService for local development.");
    }
}