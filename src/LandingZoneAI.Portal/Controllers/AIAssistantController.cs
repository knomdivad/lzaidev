using Microsoft.AspNetCore.Mvc;
using LandingZoneAI.Portal.Models;
using LandingZoneAI.Portal.Services;

namespace LandingZoneAI.Portal.Controllers;

/// <summary>
/// AI-powered chat interface for landing zone creation
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AIAssistantController : ControllerBase
{
    private readonly IAIAssistantService _aiAssistantService;
    private readonly ILogger<AIAssistantController> _logger;

    public AIAssistantController(IAIAssistantService aiAssistantService, ILogger<AIAssistantController> logger)
    {
        _aiAssistantService = aiAssistantService;
        _logger = logger;
    }

    /// <summary>
    /// Start a new AI conversation for landing zone creation
    /// </summary>
    [HttpPost("conversations")]
    public async Task<ActionResult<AIConversation>> StartConversation([FromBody] StartConversationRequest request)
    {
        try
        {
            var conversation = await _aiAssistantService.StartConversationAsync(request.CustomerId);
            return Ok(conversation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting AI conversation for customer {CustomerId}", request.CustomerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Send a message to the AI assistant
    /// </summary>
    [HttpPost("conversations/{conversationId}/messages")]
    public async Task<ActionResult<AIMessage>> SendMessage(string conversationId, [FromBody] SendMessageRequest request)
    {
        try
        {
            var response = await _aiAssistantService.SendMessageAsync(conversationId, request.Message);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message for conversation {ConversationId}", conversationId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get conversation history
    /// </summary>
    [HttpGet("conversations/{conversationId}")]
    public async Task<ActionResult<AIConversation>> GetConversation(string conversationId)
    {
        try
        {
            var conversation = await _aiAssistantService.GetConversationAsync(conversationId);
            if (conversation == null)
                return NotFound($"Conversation {conversationId} not found");

            return Ok(conversation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation {ConversationId}", conversationId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get conversations for a customer
    /// </summary>
    [HttpGet("customers/{customerId}/conversations")]
    public async Task<ActionResult<List<AIConversation>>> GetCustomerConversations(string customerId)
    {
        try
        {
            var conversations = await _aiAssistantService.GetCustomerConversationsAsync(customerId);
            return Ok(conversations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversations for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Generate landing zone recommendations based on conversation
    /// </summary>
    [HttpPost("conversations/{conversationId}/recommendations")]
    public async Task<ActionResult<LandingZoneRecommendation>> GenerateRecommendations(string conversationId)
    {
        try
        {
            var recommendation = await _aiAssistantService.GenerateRecommendationsAsync(conversationId);
            return Ok(recommendation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recommendations for conversation {ConversationId}", conversationId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Deploy landing zone based on AI recommendations
    /// </summary>
    [HttpPost("conversations/{conversationId}/deploy")]
    public async Task<ActionResult<CustomerLandingZone>> DeployFromRecommendation(string conversationId, [FromBody] DeployFromAIRequest request)
    {
        try
        {
            var landingZone = await _aiAssistantService.DeployFromRecommendationAsync(conversationId, request);
            return Ok(landingZone);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deploying landing zone from conversation {ConversationId}", conversationId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get deployment progress for AI-initiated deployment
    /// </summary>
    [HttpGet("deployments/{landingZoneId}/progress")]
    public async Task<ActionResult<DeploymentProgress>> GetDeploymentProgress(string landingZoneId)
    {
        try
        {
            var progress = await _aiAssistantService.GetDeploymentProgressAsync(landingZoneId);
            return Ok(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving deployment progress for landing zone {LandingZoneId}", landingZoneId);
            return StatusCode(500, "Internal server error");
        }
    }
}

/// <summary>
/// Request/Response models for AI Assistant
/// </summary>
public class StartConversationRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string? InitialMessage { get; set; }
}

public class SendMessageRequest
{
    public string Message { get; set; } = string.Empty;
}

public class DeployFromAIRequest
{
    public string? ProjectName { get; set; }
    public string Environment { get; set; } = "dev";
    public string? SelectedTemplateId { get; set; }
    public Dictionary<string, object>? CustomParameters { get; set; }
}

public class LandingZoneRecommendation
{
    public string ConversationId { get; set; } = string.Empty;
    public List<RecommendedTemplate> RecommendedTemplates { get; set; } = new();
    public LandingZoneRequirements Requirements { get; set; } = new();
    public CostEstimate CostEstimate { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public Dictionary<string, object> GeneratedParameters { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class RecommendedTemplate
{
    public LandingZoneTemplate Template { get; set; } = new();
    public decimal MatchScore { get; set; }
    public List<string> MatchReasons { get; set; } = new();
    public Dictionary<string, object> RecommendedParameters { get; set; } = new();
}

public class CostEstimate
{
    public decimal? EstimatedMonthlyCost { get; set; }
    public decimal? EstimatedDailyCost { get; set; }
    public Dictionary<string, decimal> CostBreakdown { get; set; } = new();
    public List<string> CostOptimizationSuggestions { get; set; } = new();
}

public class DeploymentProgress
{
    public string LandingZoneId { get; set; } = string.Empty;
    public LandingZoneStatus Status { get; set; }
    public int ProgressPercentage { get; set; }
    public string CurrentStep { get; set; } = string.Empty;
    public List<DeploymentStep> Steps { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime? EstimatedCompletionTime { get; set; }
    public string? ErrorMessage { get; set; }
}

public class DeploymentStep
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public StepStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public enum StepStatus
{
    Pending,
    InProgress,
    Completed,
    Failed,
    Skipped
}