using Microsoft.AspNetCore.Mvc;
using LandingZoneAI.Portal.Services;
using LandingZoneAI.Portal.Models;

namespace LandingZoneAI.Portal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly IAzureResourceService _resourceService;
    private readonly ILogger<ResourcesController> _logger;

    public ResourcesController(IAzureResourceService resourceService, ILogger<ResourcesController> logger)
    {
        _resourceService = resourceService;
        _logger = logger;
    }

    /// <summary>
    /// Get all resource groups in a subscription
    /// </summary>
    [HttpGet("subscriptions/{subscriptionId}/resource-groups")]
    public async Task<ActionResult<List<string>>> GetResourceGroups(string subscriptionId)
    {
        try
        {
            var resourceGroups = await _resourceService.GetResourceGroupsAsync(subscriptionId);
            return Ok(resourceGroups);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resource groups for subscription {SubscriptionId}", subscriptionId);
            return StatusCode(500, new { error = "Failed to retrieve resource groups" });
        }
    }

    /// <summary>
    /// Get all resources in a subscription or resource group
    /// </summary>
    [HttpGet("subscriptions/{subscriptionId}/resources")]
    public async Task<ActionResult<List<ResourceSummary>>> GetResources(
        string subscriptionId, 
        [FromQuery] string? resourceGroupName = null)
    {
        try
        {
            var resources = await _resourceService.GetResourcesAsync(subscriptionId, resourceGroupName);
            return Ok(resources);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resources for subscription {SubscriptionId}", subscriptionId);
            return StatusCode(500, new { error = "Failed to retrieve resources" });
        }
    }

    /// <summary>
    /// Get Machine Learning workspace information
    /// </summary>
    [HttpGet("subscriptions/{subscriptionId}/resource-groups/{resourceGroupName}/ml-workspaces/{workspaceName}")]
    public async Task<ActionResult<MLWorkspaceInfo>> GetMLWorkspace(
        string subscriptionId, 
        string resourceGroupName, 
        string workspaceName)
    {
        try
        {
            var workspace = await _resourceService.GetMLWorkspaceInfoAsync(subscriptionId, resourceGroupName, workspaceName);
            if (workspace == null)
            {
                return NotFound(new { error = "ML workspace not found" });
            }
            return Ok(workspace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ML workspace {WorkspaceName}", workspaceName);
            return StatusCode(500, new { error = "Failed to retrieve ML workspace information" });
        }
    }

    /// <summary>
    /// Get OpenAI service information
    /// </summary>
    [HttpGet("subscriptions/{subscriptionId}/resource-groups/{resourceGroupName}/openai-services/{serviceName}")]
    public async Task<ActionResult<OpenAIServiceInfo>> GetOpenAIService(
        string subscriptionId, 
        string resourceGroupName, 
        string serviceName)
    {
        try
        {
            var service = await _resourceService.GetOpenAIServiceInfoAsync(subscriptionId, resourceGroupName, serviceName);
            if (service == null)
            {
                return NotFound(new { error = "OpenAI service not found" });
            }
            return Ok(service);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting OpenAI service {ServiceName}", serviceName);
            return StatusCode(500, new { error = "Failed to retrieve OpenAI service information" });
        }
    }

    /// <summary>
    /// Get AKS cluster information
    /// </summary>
    [HttpGet("subscriptions/{subscriptionId}/resource-groups/{resourceGroupName}/aks-clusters/{clusterName}")]
    public async Task<ActionResult<AKSClusterInfo>> GetAKSCluster(
        string subscriptionId, 
        string resourceGroupName, 
        string clusterName)
    {
        try
        {
            var cluster = await _resourceService.GetAKSClusterInfoAsync(subscriptionId, resourceGroupName, clusterName);
            if (cluster == null)
            {
                return NotFound(new { error = "AKS cluster not found" });
            }
            return Ok(cluster);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AKS cluster {ClusterName}", clusterName);
            return StatusCode(500, new { error = "Failed to retrieve AKS cluster information" });
        }
    }
}