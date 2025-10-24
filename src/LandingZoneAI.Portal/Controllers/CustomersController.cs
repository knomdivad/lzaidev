using Microsoft.AspNetCore.Mvc;
using LandingZoneAI.Portal.Models;
using LandingZoneAI.Portal.Services;

namespace LandingZoneAI.Portal.Controllers;

/// <summary>
/// Customer management API controller
/// </summary>
[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        Console.WriteLine("🏗️ CustomersController constructor called");
        _customerService = customerService;
        _logger = logger;
        Console.WriteLine("✅ CustomersController initialized successfully");
    }

    /// <summary>
    /// Test endpoint for debugging
    /// </summary>
    [HttpGet("test")]
    public ActionResult<string> TestEndpoint()
    {
        Console.WriteLine("🧪 Test endpoint called");
        return Ok("CustomersController is working!");
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Customer>>> GetCustomers([FromQuery] CustomerStatus? status = null)
    {
        Console.WriteLine($"🔍 CustomersController.GetCustomers called with status: {status}");
        try
        {
            var customers = await _customerService.GetCustomersAsync(status);
            Console.WriteLine($"✅ Retrieved {customers.Count} customers");
            return Ok(customers);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in GetCustomers: {ex.Message}");
            _logger.LogError(ex, "Error retrieving customers");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{customerId}")]
    public async Task<ActionResult<Customer>> GetCustomer(string customerId)
    {
        try
        {
            var customer = await _customerService.GetCustomerAsync(customerId);
            if (customer == null)
                return NotFound($"Customer {customerId} not found");

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create new customer
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        try
        {
            var customer = await _customerService.CreateCustomerAsync(request);
            return CreatedAtAction(nameof(GetCustomer), new { customerId = customer.Id }, customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update customer
    /// </summary>
    [HttpPut("{customerId}")]
    public async Task<ActionResult<Customer>> UpdateCustomer(string customerId, [FromBody] UpdateCustomerRequest request)
    {
        try
        {
            var customer = await _customerService.UpdateCustomerAsync(customerId, request);
            if (customer == null)
                return NotFound($"Customer {customerId} not found");

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Configure cloud provider for customer
    /// </summary>
    [HttpPost("{customerId}/cloud-providers")]
    public async Task<ActionResult<CloudProvider>> ConfigureCloudProvider(string customerId, [FromBody] CloudProviderConfiguration config)
    {
        try
        {
            var cloudProvider = await _customerService.ConfigureCloudProviderAsync(customerId, config);
            return Ok(cloudProvider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring cloud provider for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Configure source control provider for customer
    /// </summary>
    [HttpPost("{customerId}/source-control-providers")]
    public async Task<ActionResult<SourceControlProvider>> ConfigureSourceControlProvider(string customerId, [FromBody] SourceControlConfiguration config)
    {
        try
        {
            var sourceControlProvider = await _customerService.ConfigureSourceControlProviderAsync(customerId, config);
            return Ok(sourceControlProvider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring source control provider for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get customer's landing zones
    /// </summary>
    [HttpGet("{customerId}/landing-zones")]
    public async Task<ActionResult<List<CustomerLandingZone>>> GetCustomerLandingZones(string customerId)
    {
        try
        {
            var landingZones = await _customerService.GetCustomerLandingZonesAsync(customerId);
            return Ok(landingZones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving landing zones for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get available templates for customer
    /// </summary>
    [HttpGet("{customerId}/templates")]
    public async Task<ActionResult<List<LandingZoneTemplate>>> GetAvailableTemplates(string customerId)
    {
        try
        {
            var templates = await _customerService.GetAvailableTemplatesAsync(customerId);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving templates for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }
}

/// <summary>
/// Request models for customer operations
/// </summary>
public class CreateCustomerRequest
{
    public string Name { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
}

public class UpdateCustomerRequest
{
    public string? Name { get; set; }
    public string? ContactEmail { get; set; }
    public string? CompanyName { get; set; }
    public CustomerStatus? Status { get; set; }
}

public class CloudProviderConfiguration
{
    public CloudProviderType Type { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public Dictionary<string, string> Configuration { get; set; } = new();
    public bool IsDefault { get; set; }
}

public class SourceControlConfiguration
{
    public SourceControlType Type { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? RepositoryUrl { get; set; }
    public string? Organization { get; set; }
    public Dictionary<string, string> Configuration { get; set; } = new();
    public bool IsDefault { get; set; }
}