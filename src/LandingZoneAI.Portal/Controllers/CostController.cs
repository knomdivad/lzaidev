using Microsoft.AspNetCore.Mvc;
using LandingZoneAI.Portal.Services;
using LandingZoneAI.Portal.Models;

namespace LandingZoneAI.Portal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CostController : ControllerBase
{
    private readonly ICostManagementService _costService;
    private readonly ILogger<CostController> _logger;

    public CostController(ICostManagementService costService, ILogger<CostController> logger)
    {
        _costService = costService;
        _logger = logger;
    }

    /// <summary>
    /// Get cost data for resources
    /// </summary>
    [HttpGet("resources/{subscriptionId}")]
    public async Task<ActionResult<List<CostData>>> GetResourceCosts(
        string subscriptionId,
        [FromQuery] string? resourceGroupName = null,
        [FromQuery] int days = 30)
    {
        try
        {
            var period = TimeSpan.FromDays(days);
            var costs = await _costService.GetResourceCostsAsync(subscriptionId, resourceGroupName, period);
            return Ok(costs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resource costs for subscription {SubscriptionId}", subscriptionId);
            return StatusCode(500, new { error = "Failed to retrieve resource costs" });
        }
    }

    /// <summary>
    /// Get projected monthly cost
    /// </summary>
    [HttpGet("projected-monthly/{subscriptionId}")]
    public async Task<ActionResult<object>> GetProjectedMonthlyCost(
        string subscriptionId,
        [FromQuery] string? resourceGroupName = null)
    {
        try
        {
            var projectedCost = await _costService.GetProjectedMonthlyCostAsync(subscriptionId, resourceGroupName);
            return Ok(new { projectedMonthlyCost = projectedCost, currency = "USD", lastUpdated = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projected monthly cost");
            return StatusCode(500, new { error = "Failed to retrieve projected monthly cost" });
        }
    }

    /// <summary>
    /// Get cost breakdown by resource type
    /// </summary>
    [HttpGet("by-resource-type/{subscriptionId}")]
    public async Task<ActionResult<Dictionary<string, decimal>>> GetCostByResourceType(
        string subscriptionId,
        [FromQuery] string? resourceGroupName = null)
    {
        try
        {
            var costByType = await _costService.GetCostByResourceTypeAsync(subscriptionId, resourceGroupName);
            return Ok(costByType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cost by resource type");
            return StatusCode(500, new { error = "Failed to retrieve cost breakdown by resource type" });
        }
    }

    /// <summary>
    /// Get daily cost trend
    /// </summary>
    [HttpGet("daily-trend/{subscriptionId}")]
    public async Task<ActionResult<List<CostData>>> GetDailyCostTrend(
        string subscriptionId,
        [FromQuery] string? resourceGroupName = null,
        [FromQuery] int days = 30)
    {
        try
        {
            var trend = await _costService.GetDailyCostTrendAsync(subscriptionId, resourceGroupName, days);
            return Ok(trend);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting daily cost trend");
            return StatusCode(500, new { error = "Failed to retrieve daily cost trend" });
        }
    }

    /// <summary>
    /// Get cost summary dashboard data
    /// </summary>
    [HttpGet("summary/{subscriptionId}")]
    public async Task<ActionResult<object>> GetCostSummary(
        string subscriptionId,
        [FromQuery] string? resourceGroupName = null)
    {
        try
        {
            var tasks = new Task[]
            {
                _costService.GetResourceCostsAsync(subscriptionId, resourceGroupName),
                _costService.GetProjectedMonthlyCostAsync(subscriptionId, resourceGroupName),
                _costService.GetCostByResourceTypeAsync(subscriptionId, resourceGroupName),
                _costService.GetDailyCostTrendAsync(subscriptionId, resourceGroupName, 7) // Last 7 days
            };

            await Task.WhenAll(tasks);

            var resourceCosts = await _costService.GetResourceCostsAsync(subscriptionId, resourceGroupName);
            var projectedCost = await _costService.GetProjectedMonthlyCostAsync(subscriptionId, resourceGroupName);
            var costByType = await _costService.GetCostByResourceTypeAsync(subscriptionId, resourceGroupName);
            var weeklyTrend = await _costService.GetDailyCostTrendAsync(subscriptionId, resourceGroupName, 7);

            var summary = new
            {
                totalDailyCost = resourceCosts.Sum(c => c.DailyCost),
                totalMonthlyCost = resourceCosts.Sum(c => c.MonthlyCost),
                projectedMonthlyCost = projectedCost,
                costByResourceType = costByType,
                weeklyTrend = weeklyTrend.TakeLast(7).ToList(),
                resourceCount = resourceCosts.Count,
                lastUpdated = DateTime.UtcNow
            };

            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cost summary");
            return StatusCode(500, new { error = "Failed to retrieve cost summary" });
        }
    }
}