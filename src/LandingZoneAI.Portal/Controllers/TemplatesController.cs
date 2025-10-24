using Microsoft.AspNetCore.Mvc;
using LandingZoneAI.Portal.Models;
using LandingZoneAI.Portal.Services;

namespace LandingZoneAI.Portal.Controllers;

/// <summary>
/// Template management API controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    private readonly ITemplateManagementService _templateService;
    private readonly ILogger<TemplatesController> _logger;

    public TemplatesController(ITemplateManagementService templateService, ILogger<TemplatesController> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    /// <summary>
    /// Get all landing zone templates
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<LandingZoneTemplate>>> GetTemplates()
    {
        try
        {
            var templates = await _templateService.GetTemplatesAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving templates");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get template by ID
    /// </summary>
    [HttpGet("{templateId}")]
    public async Task<ActionResult<LandingZoneTemplate>> GetTemplate(string templateId)
    {
        try
        {
            var template = await _templateService.GetTemplateAsync(templateId);
            if (template == null)
                return NotFound($"Template {templateId} not found");

            return Ok(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template {TemplateId}", templateId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create new template
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LandingZoneTemplate>> CreateTemplate([FromBody] CreateTemplateRequest request)
    {
        try
        {
            var template = await _templateService.CreateTemplateAsync(request);
            return CreatedAtAction(nameof(GetTemplate), new { templateId = template.Id }, template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating template");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update existing template
    /// </summary>
    [HttpPut("{templateId}")]
    public async Task<ActionResult<LandingZoneTemplate>> UpdateTemplate(string templateId, [FromBody] UpdateTemplateRequest request)
    {
        try
        {
            var template = await _templateService.UpdateTemplateAsync(templateId, request);
            if (template == null)
                return NotFound($"Template {templateId} not found");

            return Ok(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {TemplateId}", templateId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete template
    /// </summary>
    [HttpDelete("{templateId}")]
    public async Task<ActionResult> DeleteTemplate(string templateId)
    {
        try
        {
            var success = await _templateService.DeleteTemplateAsync(templateId);
            if (!success)
                return NotFound($"Template {templateId} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", templateId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get templates by category
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<LandingZoneTemplate>>> GetTemplatesByCategory(TemplateType category)
    {
        try
        {
            var templates = await _templateService.GetTemplatesByCategoryAsync(category);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving templates by category {Category}", category);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get templates by cloud provider
    /// </summary>
    [HttpGet("cloud-provider/{cloudProvider}")]
    public async Task<ActionResult<List<LandingZoneTemplate>>> GetTemplatesByCloudProvider(CloudProviderType cloudProvider)
    {
        try
        {
            var templates = await _templateService.GetTemplatesByCloudProviderAsync(cloudProvider);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving templates by cloud provider {CloudProvider}", cloudProvider);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Clone an existing template
    /// </summary>
    [HttpPost("{templateId}/clone")]
    public async Task<ActionResult<LandingZoneTemplate>> CloneTemplate(string templateId, [FromBody] CloneTemplateRequest request)
    {
        try
        {
            var clonedTemplate = await _templateService.CloneTemplateAsync(templateId, request.NewName);
            if (clonedTemplate == null)
                return NotFound($"Template {templateId} not found");

            return CreatedAtAction(nameof(GetTemplate), new { templateId = clonedTemplate.Id }, clonedTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cloning template {TemplateId}", templateId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Validate template configuration
    /// </summary>
    [HttpPost("{templateId}/validate")]
    public async Task<ActionResult<TemplateValidationResult>> ValidateTemplate(string templateId)
    {
        try
        {
            var template = await _templateService.GetTemplateAsync(templateId);
            if (template == null)
                return NotFound($"Template {templateId} not found");

            var isValid = await _templateService.ValidateTemplateAsync(template);
            
            return Ok(new TemplateValidationResult
            {
                TemplateId = templateId,
                IsValid = isValid,
                ValidationDate = DateTime.UtcNow,
                Errors = isValid ? new List<string>() : new List<string> { "Validation failed - check logs for details" }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating template {TemplateId}", templateId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get template default parameters
    /// </summary>
    [HttpGet("{templateId}/parameters")]
    public async Task<ActionResult<Dictionary<string, object>>> GetTemplateParameters(string templateId)
    {
        try
        {
            var parameters = await _templateService.GetTemplateDefaultParametersAsync(templateId);
            return Ok(parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parameters for template {TemplateId}", templateId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get template categories
    /// </summary>
    [HttpGet("categories")]
    public ActionResult<List<TemplateCategoryInfo>> GetTemplateCategories()
    {
        var categories = new List<TemplateCategoryInfo>
        {
            new TemplateCategoryInfo
            {
                Type = TemplateType.BasicAI,
                Name = "Basic AI Development",
                Description = "Simple AI development environments for small teams and proof-of-concept projects",
                Icon = "🚀",
                EstimatedCostRange = new CostRange { Min = 200m, Max = 800m }
            },
            new TemplateCategoryInfo
            {
                Type = TemplateType.EnterpriseAI,
                Name = "Enterprise AI Platform", 
                Description = "Production-ready AI platforms with enterprise security and compliance",
                Icon = "🏢",
                EstimatedCostRange = new CostRange { Min = 1500m, Max = 5000m }
            },
            new TemplateCategoryInfo
            {
                Type = TemplateType.MLOps,
                Name = "MLOps Pipeline",
                Description = "Complete machine learning operations with CI/CD and model management",
                Icon = "⚙️",
                EstimatedCostRange = new CostRange { Min = 800m, Max = 2500m }
            },
            new TemplateCategoryInfo
            {
                Type = TemplateType.DataScience,
                Name = "Data Science Workspace",
                Description = "Collaborative environments for data science teams and research",
                Icon = "📊",
                EstimatedCostRange = new CostRange { Min = 400m, Max = 1500m }
            },
            new TemplateCategoryInfo
            {
                Type = TemplateType.CustomAI,
                Name = "Custom AI Solution",
                Description = "Flexible templates for specialized AI use cases and requirements",
                Icon = "🎨",
                EstimatedCostRange = new CostRange { Min = 300m, Max = 3000m }
            }
        };

        return Ok(categories);
    }
}

/// <summary>
/// Request/Response models for template operations
/// </summary>
public class CloneTemplateRequest
{
    public string NewName { get; set; } = string.Empty;
}

public class TemplateValidationResult
{
    public string TemplateId { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public DateTime ValidationDate { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class TemplateCategoryInfo
{
    public TemplateType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public CostRange EstimatedCostRange { get; set; } = new();
}

public class CostRange
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }
}