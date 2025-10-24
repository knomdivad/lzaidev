using LandingZoneAI.Portal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Landing Zone AI Portal API", Version = "v1" });
});

// Always use mock services for local development
builder.Services.AddScoped<IAzureResourceService, MockAzureResourceService>();
builder.Services.AddScoped<IAIServicesMonitoringService, AIServicesMonitoringService>();
builder.Services.AddScoped<ICostManagementService, CostManagementService>();
builder.Services.AddScoped<IDeploymentService, DeploymentService>();

// Customer management services
builder.Services.AddScoped<ICustomerService, MockCustomerService>();
builder.Services.AddScoped<IAIAssistantService, MockAIAssistantService>();
builder.Services.AddScoped<ITemplateManagementService, MockTemplateManagementService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3001") // React dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Debug: Log all registered controllers
Console.WriteLine("🔍 Registered Controllers:");
var feature = new Microsoft.AspNetCore.Mvc.Controllers.ControllerFeature();
app.Services.GetRequiredService<Microsoft.AspNetCore.Mvc.ApplicationParts.ApplicationPartManager>().PopulateFeature(feature);
foreach (var controller in feature.Controllers)
{
    Console.WriteLine($"  - {controller.FullName}");
}

// Configure the HTTP request pipeline.
// Enable Swagger in all environments for local testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Landing Zone AI Portal API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowFrontend");

// Debug: Add manual test endpoint to verify routing works
app.MapGet("/api/test", () => Results.Ok(new { message = "Manual API test works" }));

app.MapControllers();

// Debug: Log routing information
var endpointDataSource = app.Services.GetRequiredService<Microsoft.AspNetCore.Routing.EndpointDataSource>();
Console.WriteLine("🗺️ Registered Routes:");
foreach (var endpoint in endpointDataSource.Endpoints)
{
    Console.WriteLine($"  - {endpoint.DisplayName}");
}

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Root endpoint
app.MapGet("/", () => Results.Ok(new { 
    service = "Landing Zone AI Portal API", 
    version = "1.0.0",
    environment = app.Environment.EnvironmentName,
    mode = "Mock Data Mode - No Azure Dependencies"
}));

Console.WriteLine("🚀 Starting Landing Zone AI Portal");
Console.WriteLine("📍 Environment: " + app.Environment.EnvironmentName);
Console.WriteLine("🔧 Mode: Mock Data (No Azure Dependencies)");
Console.WriteLine("📊 Swagger UI: http://localhost:5000/swagger");
Console.WriteLine("🔗 API Base: http://localhost:5000/api");

app.Run();