using Microsoft.AspNetCore.Mvc;
using LandingZoneAI.Portal.Services;
using LandingZoneAI.Portal.Models;

namespace LandingZoneAI.Portal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeploymentController : ControllerBase
{
    private readonly IDeploymentService _deploymentService;
    private readonly ILogger<DeploymentController> _logger;

    public DeploymentController(IDeploymentService deploymentService, ILogger<DeploymentController> logger)
    {
        _deploymentService = deploymentService;
        _logger = logger;
    }

    /// <summary>
    /// Deploy a new AI Landing Zone
    /// </summary>
    [HttpPost("deploy")]
    public async Task<ActionResult<DeploymentStatus>> DeployLandingZone([FromBody] DeploymentRequest request)
    {
        try
        {
            // Validate the deployment request
            var isValid = await _deploymentService.ValidateDeploymentAsync(request);
            if (!isValid)
            {
                return BadRequest(new { error = "Invalid deployment request. Please check your parameters." });
            }

            var deployment = await _deploymentService.DeployLandingZoneAsync(request);
            return Accepted(deployment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting deployment for project {ProjectName}", request.ProjectName);
            return StatusCode(500, new { error = "Failed to start deployment" });
        }
    }

    /// <summary>
    /// Get deployment status
    /// </summary>
    [HttpGet("status/{deploymentId}")]
    public async Task<ActionResult<DeploymentStatus>> GetDeploymentStatus(string deploymentId)
    {
        try
        {
            var status = await _deploymentService.GetDeploymentStatusAsync(deploymentId);
            return Ok(status);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Deployment not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting deployment status for {DeploymentId}", deploymentId);
            return StatusCode(500, new { error = "Failed to retrieve deployment status" });
        }
    }

    /// <summary>
    /// Get recent deployments
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<List<DeploymentStatus>>> GetRecentDeployments([FromQuery] int count = 10)
    {
        try
        {
            var deployments = await _deploymentService.GetRecentDeploymentsAsync(count);
            return Ok(deployments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent deployments");
            return StatusCode(500, new { error = "Failed to retrieve recent deployments" });
        }
    }

    /// <summary>
    /// Validate deployment parameters
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<object>> ValidateDeployment([FromBody] DeploymentRequest request)
    {
        try
        {
            var isValid = await _deploymentService.ValidateDeploymentAsync(request);
            return Ok(new { isValid, validatedAt = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating deployment request");
            return StatusCode(500, new { error = "Failed to validate deployment request" });
        }
    }

    /// <summary>
    /// Get deployment template with default parameters
    /// </summary>
    [HttpGet("template")]
    public ActionResult<object> GetDeploymentTemplate()
    {
        var template = new
        {
            projectName = "my-ai-project",
            environment = "dev",
            location = "eastus",
            parameters = new Dictionary<string, object>
            {
                ["enablePrivateEndpoints"] = true,
                ["aksNodeCount"] = 3,
                ["aksVmSize"] = "Standard_D4s_v3",
                ["aiNodeCount"] = 2,
                ["aiVmSize"] = "Standard_NC4as_T4_v3",
                ["storageAccountSku"] = "Standard_LRS",
                ["openaiModels"] = new[]
                {
                    new { name = "gpt-35-turbo", capacity = 30 },
                    new { name = "gpt-4", capacity = 10 },
                    new { name = "text-embedding-ada-002", capacity = 30 }
                }
            }
        };

        return Ok(template);
    }
}