using Microsoft.AspNetCore.Mvc;
using LandingZoneAI.Portal.Services;
using LandingZoneAI.Portal.Models;

namespace LandingZoneAI.Portal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonitoringController : ControllerBase
{
    private readonly IAIServicesMonitoringService _monitoringService;
    private readonly ILogger<MonitoringController> _logger;

    public MonitoringController(IAIServicesMonitoringService monitoringService, ILogger<MonitoringController> logger)
    {
        _monitoringService = monitoringService;
        _logger = logger;
    }

    /// <summary>
    /// Get OpenAI usage metrics
    /// </summary>
    [HttpGet("openai-usage/{subscriptionId}/{resourceGroupName}")]
    public async Task<ActionResult<List<UsageMetrics>>> GetOpenAIUsage(
        string subscriptionId, 
        string resourceGroupName,
        [FromQuery] int hours = 24)
    {
        try
        {
            var period = TimeSpan.FromHours(hours);
            var usage = await _monitoringService.GetOpenAIUsageAsync(subscriptionId, resourceGroupName, period);
            return Ok(usage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting OpenAI usage metrics");
            return StatusCode(500, new { error = "Failed to retrieve OpenAI usage metrics" });
        }
    }

    /// <summary>
    /// Get AKS cluster metrics
    /// </summary>
    [HttpGet("aks-metrics/{subscriptionId}/{resourceGroupName}/{clusterName}")]
    public async Task<ActionResult<List<ClusterMetrics>>> GetAKSMetrics(
        string subscriptionId, 
        string resourceGroupName, 
        string clusterName,
        [FromQuery] int hours = 24)
    {
        try
        {
            var period = TimeSpan.FromHours(hours);
            var metrics = await _monitoringService.GetAKSMetricsAsync(subscriptionId, resourceGroupName, clusterName, period);
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AKS metrics for cluster {ClusterName}", clusterName);
            return StatusCode(500, new { error = "Failed to retrieve AKS metrics" });
        }
    }

    /// <summary>
    /// Get Machine Learning workspace metrics
    /// </summary>
    [HttpGet("ml-workspace-metrics/{subscriptionId}/{resourceGroupName}/{workspaceName}")]
    public async Task<ActionResult<Dictionary<string, object>>> GetMLWorkspaceMetrics(
        string subscriptionId, 
        string resourceGroupName, 
        string workspaceName,
        [FromQuery] int hours = 24)
    {
        try
        {
            var period = TimeSpan.FromHours(hours);
            var metrics = await _monitoringService.GetMLWorkspaceMetricsAsync(subscriptionId, resourceGroupName, workspaceName, period);
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ML workspace metrics for {WorkspaceName}", workspaceName);
            return StatusCode(500, new { error = "Failed to retrieve ML workspace metrics" });
        }
    }

    /// <summary>
    /// Get health status of all AI services
    /// </summary>
    [HttpGet("health-status/{subscriptionId}/{resourceGroupName}")]
    public async Task<ActionResult<object>> GetHealthStatus(string subscriptionId, string resourceGroupName)
    {
        try
        {
            // This would aggregate health status from all services
            await Task.Delay(100); // Simulate async operation
            
            var healthStatus = new
            {
                overall = "Healthy",
                services = new
                {
                    openai = new { status = "Healthy", lastCheck = DateTime.UtcNow },
                    mlWorkspace = new { status = "Healthy", lastCheck = DateTime.UtcNow },
                    aksCluster = new { status = "Healthy", lastCheck = DateTime.UtcNow },
                    storage = new { status = "Healthy", lastCheck = DateTime.UtcNow },
                    keyVault = new { status = "Healthy", lastCheck = DateTime.UtcNow }
                },
                lastUpdated = DateTime.UtcNow
            };

            return Ok(healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting health status");
            return StatusCode(500, new { error = "Failed to retrieve health status" });
        }
    }
}