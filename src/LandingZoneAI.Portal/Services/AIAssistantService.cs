using LandingZoneAI.Portal.Models;
using LandingZoneAI.Portal.Controllers;
using System.Text.Json;

namespace LandingZoneAI.Portal.Services;

public interface IAIAssistantService
{
    Task<AIConversation> StartConversationAsync(string customerId);
    Task<AIMessage> SendMessageAsync(string conversationId, string message);
    Task<AIConversation?> GetConversationAsync(string conversationId);
    Task<List<AIConversation>> GetCustomerConversationsAsync(string customerId);
    Task<LandingZoneRecommendation> GenerateRecommendationsAsync(string conversationId);
    Task<CustomerLandingZone> DeployFromRecommendationAsync(string conversationId, DeployFromAIRequest request);
    Task<DeploymentProgress> GetDeploymentProgressAsync(string landingZoneId);
}

public class MockAIAssistantService : IAIAssistantService
{
    private readonly ILogger<MockAIAssistantService> _logger;
    private readonly ICustomerService _customerService;
    private readonly IDeploymentService _deploymentService;
    private static readonly List<AIConversation> _conversations = new();
    private static readonly Dictionary<string, DeploymentProgress> _deploymentProgress = new();

    // AI response templates for different conversation stages
    private static readonly Dictionary<string, List<string>> _aiResponses = new()
    {
        ["greeting"] = new List<string>
        {
            "Hello! I'm your AI assistant for creating Azure AI Landing Zones. I'll help you design the perfect AI infrastructure for your needs. To get started, could you tell me about your project? What kind of AI workloads are you planning to run?",
            "Hi there! I'm here to help you set up an AI Landing Zone tailored to your requirements. Let's begin by understanding your use case - are you working on machine learning, natural language processing, computer vision, or something else?",
            "Welcome! I'll guide you through creating a custom AI Landing Zone. To recommend the best setup, I'd like to know: What's your experience level with cloud AI services, and what's the main goal of your AI project?"
        },
        ["requirements"] = new List<string>
        {
            "That sounds like an interesting project! To recommend the right infrastructure, I need to understand a few more details:",
            "Great! Based on what you've told me, I can suggest some options. Let me ask a few more questions to fine-tune the recommendations:",
            "Perfect! I'm getting a clearer picture of your needs. A few more questions to ensure we design the optimal solution:"
        },
        ["recommendation"] = new List<string>
        {
            "Based on our conversation, I've analyzed your requirements and have some excellent recommendations for your AI Landing Zone.",
            "Thank you for providing all those details! I've processed your requirements and found the perfect template matches for your use case.",
            "Great! I've compiled everything you've told me and generated personalized recommendations that should meet your needs perfectly."
        }
    };

    public MockAIAssistantService(
        ILogger<MockAIAssistantService> logger, 
        ICustomerService customerService,
        IDeploymentService deploymentService)
    {
        _logger = logger;
        _customerService = customerService;
        _deploymentService = deploymentService;
    }

    public Task<AIConversation> StartConversationAsync(string customerId)
    {
        _logger.LogInformation("Mock AI: Starting conversation for customer {CustomerId}", customerId);

        var conversation = new AIConversation
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = customerId,
            Status = ConversationStatus.Active,
            Context = new Dictionary<string, object>
            {
                ["stage"] = "greeting",
                ["requirements"] = new LandingZoneRequirements()
            }
        };

        // Add initial AI greeting
        var greetingMessage = new AIMessage
        {
            Type = MessageType.AIResponse,
            Content = GetRandomResponse("greeting"),
            Metadata = new Dictionary<string, object>
            {
                ["stage"] = "greeting",
                ["suggestedQuestions"] = new List<string>
                {
                    "What type of AI project are you working on?",
                    "Do you need GPU compute for machine learning?", 
                    "What's your expected monthly budget?",
                    "How many team members will be using this environment?"
                }
            }
        };

        conversation.Messages.Add(greetingMessage);
        _conversations.Add(conversation);

        return Task.FromResult(conversation);
    }

    public Task<AIMessage> SendMessageAsync(string conversationId, string message)
    {
        _logger.LogInformation("Mock AI: Processing message for conversation {ConversationId}", conversationId);

        var conversation = _conversations.FirstOrDefault(c => c.Id == conversationId);
        if (conversation == null)
            throw new ArgumentException($"Conversation {conversationId} not found");

        // Add user message
        var userMessage = new AIMessage
        {
            Type = MessageType.UserMessage,
            Content = message
        };
        conversation.Messages.Add(userMessage);

        // Process message and extract requirements
        var requirements = ExtractRequirementsFromMessage(message, conversation);
        conversation.Context["requirements"] = requirements;
        
        // Determine conversation stage and generate appropriate response
        var stage = DetermineConversationStage(conversation, requirements);
        conversation.Context["stage"] = stage;

        var aiResponse = GenerateAIResponse(conversation, requirements, stage);
        conversation.Messages.Add(aiResponse);
        conversation.LastActivityAt = DateTime.UtcNow;

        return Task.FromResult(aiResponse);
    }

    public Task<AIConversation?> GetConversationAsync(string conversationId)
    {
        _logger.LogInformation("Mock AI: Getting conversation {ConversationId}", conversationId);
        
        var conversation = _conversations.FirstOrDefault(c => c.Id == conversationId);
        return Task.FromResult(conversation);
    }

    public Task<List<AIConversation>> GetCustomerConversationsAsync(string customerId)
    {
        _logger.LogInformation("Mock AI: Getting conversations for customer {CustomerId}", customerId);
        
        var conversations = _conversations.Where(c => c.CustomerId == customerId).ToList();
        return Task.FromResult(conversations);
    }

    public async Task<LandingZoneRecommendation> GenerateRecommendationsAsync(string conversationId)
    {
        _logger.LogInformation("Mock AI: Generating recommendations for conversation {ConversationId}", conversationId);

        var conversation = _conversations.FirstOrDefault(c => c.Id == conversationId);
        if (conversation == null)
            throw new ArgumentException($"Conversation {conversationId} not found");

        var requirements = conversation.Context.ContainsKey("requirements") 
            ? JsonSerializer.Deserialize<LandingZoneRequirements>(JsonSerializer.Serialize(conversation.Context["requirements"]))
            : new LandingZoneRequirements();

        var templates = await _customerService.GetAvailableTemplatesAsync(conversation.CustomerId);
        var recommendedTemplates = new List<RecommendedTemplate>();

        // Score templates based on requirements
        foreach (var template in templates)
        {
            var score = CalculateTemplateScore(template, requirements);
            if (score > 0.3m) // Only include templates with >30% match
            {
                recommendedTemplates.Add(new RecommendedTemplate
                {
                    Template = template,
                    MatchScore = score,
                    MatchReasons = GenerateMatchReasons(template, requirements),
                    RecommendedParameters = GenerateRecommendedParameters(template, requirements)
                });
            }
        }

        // Sort by match score
        recommendedTemplates = recommendedTemplates.OrderByDescending(rt => rt.MatchScore).ToList();

        var recommendation = new LandingZoneRecommendation
        {
            ConversationId = conversationId,
            RecommendedTemplates = recommendedTemplates,
            Requirements = requirements,
            CostEstimate = CalculateCostEstimate(recommendedTemplates, requirements),
            GeneratedParameters = GenerateParameters(requirements),
            Warnings = GenerateWarnings(requirements)
        };

        return recommendation;
    }

    public async Task<CustomerLandingZone> DeployFromRecommendationAsync(string conversationId, DeployFromAIRequest request)
    {
        _logger.LogInformation("Mock AI: Deploying landing zone from conversation {ConversationId}", conversationId);

        var conversation = _conversations.FirstOrDefault(c => c.Id == conversationId);
        if (conversation == null)
            throw new ArgumentException($"Conversation {conversationId} not found");

        var customer = await _customerService.GetCustomerAsync(conversation.CustomerId);
        if (customer == null)
            throw new ArgumentException($"Customer {conversation.CustomerId} not found");

        var landingZone = new CustomerLandingZone
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = conversation.CustomerId,
            Name = request.ProjectName ?? "AI Project",
            Environment = request.Environment,
            TemplateId = request.SelectedTemplateId ?? "template-basic-ai",
            CloudProviderId = customer.CloudProviders.FirstOrDefault(cp => cp.IsDefault)?.Id ?? "",
            Status = LandingZoneStatus.Provisioning,
            Parameters = request.CustomParameters ?? new Dictionary<string, object>(),
            CreatedAt = DateTime.UtcNow
        };

        // Add to customer's landing zones
        customer.LandingZones.Add(landingZone);
        conversation.LandingZoneId = landingZone.Id;
        conversation.Status = ConversationStatus.Completed;

        // Initialize deployment progress
        InitializeDeploymentProgress(landingZone.Id);

        return landingZone;
    }

    public Task<DeploymentProgress> GetDeploymentProgressAsync(string landingZoneId)
    {
        _logger.LogInformation("Mock AI: Getting deployment progress for {LandingZoneId}", landingZoneId);

        if (!_deploymentProgress.ContainsKey(landingZoneId))
        {
            InitializeDeploymentProgress(landingZoneId);
        }

        var progress = _deploymentProgress[landingZoneId];
        
        // Simulate progress advancement
        if (progress.Status == LandingZoneStatus.Provisioning)
        {
            progress.ProgressPercentage = Math.Min(100, progress.ProgressPercentage + new Random().Next(5, 15));
            
            if (progress.ProgressPercentage >= 100)
            {
                progress.Status = LandingZoneStatus.Deployed;
                progress.CurrentStep = "Deployment Complete";
                progress.EstimatedCompletionTime = DateTime.UtcNow;
            }
            else
            {
                var steps = new[] { "Validating Configuration", "Creating Resource Group", "Deploying Network", "Setting up Storage", "Configuring AI Services", "Finalizing Security" };
                var currentStepIndex = (progress.ProgressPercentage / 16) % steps.Length;
                progress.CurrentStep = steps[currentStepIndex];
            }
        }

        return Task.FromResult(progress);
    }

    private LandingZoneRequirements ExtractRequirementsFromMessage(string message, AIConversation conversation)
    {
        var requirements = conversation.Context.ContainsKey("requirements") 
            ? JsonSerializer.Deserialize<LandingZoneRequirements>(JsonSerializer.Serialize(conversation.Context["requirements"]))
            : new LandingZoneRequirements();

        var messageLower = message.ToLowerInvariant();

        // Extract project info
        if (messageLower.Contains("project") && requirements.ProjectName == null)
        {
            requirements.ProjectName = "AI Project " + DateTime.Now.ToString("yyyyMMdd");
        }

        // Extract AI services
        if (messageLower.Contains("machine learning") || messageLower.Contains("ml"))
            requirements.RequiredServices.Add("Machine Learning");
        if (messageLower.Contains("computer vision") || messageLower.Contains("vision"))
            requirements.RequiredServices.Add("Computer Vision");
        if (messageLower.Contains("nlp") || messageLower.Contains("natural language"))
            requirements.RequiredServices.Add("Natural Language Processing");
        if (messageLower.Contains("openai") || messageLower.Contains("gpt"))
            requirements.RequiredServices.Add("OpenAI Service");

        // Extract compute requirements
        if (messageLower.Contains("gpu") || messageLower.Contains("graphics"))
            requirements.RequiresGPU = true;
        
        // Extract cost constraints
        if (messageLower.Contains("budget") || messageLower.Contains("cost"))
        {
            var costMatches = System.Text.RegularExpressions.Regex.Matches(messageLower, @"\$?(\d+(?:,\d{3})*(?:\.\d{2})?)");
            if (costMatches.Any())
            {
                if (decimal.TryParse(costMatches[0].Groups[1].Value.Replace(",", ""), out var cost))
                {
                    requirements.CostConstraints ??= new CostConstraints();
                    requirements.CostConstraints.MaxMonthlyCost = cost;
                }
            }
        }

        // Extract environment preference
        if (messageLower.Contains("development") || messageLower.Contains("dev"))
            requirements.Environment = "dev";
        else if (messageLower.Contains("production") || messageLower.Contains("prod"))
            requirements.Environment = "prod";
        else if (messageLower.Contains("staging") || messageLower.Contains("test"))
            requirements.Environment = "staging";

        return requirements;
    }

    private string DetermineConversationStage(AIConversation conversation, LandingZoneRequirements requirements)
    {
        var messageCount = conversation.Messages.Count(m => m.Type == MessageType.UserMessage);
        
        if (messageCount <= 1)
            return "greeting";
        
        // Check if we have enough info for recommendations
        var hasBasicInfo = !string.IsNullOrEmpty(requirements.ProjectName) || 
                          requirements.RequiredServices.Any() ||
                          requirements.RequiresGPU.HasValue;
        
        if (messageCount >= 3 && hasBasicInfo)
            return "recommendation";
            
        return "requirements";
    }

    private AIMessage GenerateAIResponse(AIConversation conversation, LandingZoneRequirements requirements, string stage)
    {
        var response = new AIMessage
        {
            Type = MessageType.AIResponse,
            Metadata = new Dictionary<string, object> { ["stage"] = stage }
        };

        switch (stage)
        {
            case "greeting":
                response.Content = GetRandomResponse("greeting");
                response.Metadata["suggestedQuestions"] = new List<string>
                {
                    "What type of AI project are you working on?",
                    "Do you need GPU compute for machine learning?",
                    "What's your expected monthly budget?"
                };
                break;

            case "requirements":
                response.Content = GetRandomResponse("requirements");
                
                var questions = new List<string>();
                if (string.IsNullOrEmpty(requirements.ProjectName))
                    questions.Add("What would you like to name your project?");
                if (!requirements.RequiredServices.Any())
                    questions.Add("What AI services do you need? (ML, Computer Vision, NLP, etc.)");
                if (!requirements.RequiresGPU.HasValue)
                    questions.Add("Will you need GPU compute for training models?");
                if (requirements.CostConstraints?.MaxMonthlyCost == null)
                    questions.Add("What's your monthly budget for this infrastructure?");
                if (string.IsNullOrEmpty(requirements.Environment))
                    questions.Add("Is this for development, staging, or production?");

                response.Content += "\n\n" + string.Join("\n", questions.Take(2).Select(q => "• " + q));
                response.Metadata["suggestedQuestions"] = questions;
                break;

            case "recommendation":
                response.Content = GetRandomResponse("recommendation") + 
                    "\n\nI'll analyze your requirements and provide personalized template recommendations. " +
                    "Would you like me to generate the recommendations now?";
                response.Metadata["canGenerateRecommendations"] = true;
                break;

            default:
                response.Content = "I'm here to help you create the perfect AI Landing Zone. What would you like to know?";
                break;
        }

        return response;
    }

    private decimal CalculateTemplateScore(LandingZoneTemplate template, LandingZoneRequirements requirements)
    {
        decimal score = 0.5m; // Base score

        // Match required services
        if (requirements.RequiredServices.Any())
        {
            var matchingFeatures = template.RequiredFeatures.Intersect(requirements.RequiredServices).Count();
            score += (decimal)matchingFeatures / Math.Max(template.RequiredFeatures.Count, requirements.RequiredServices.Count) * 0.3m;
        }

        // Match GPU requirements
        if (requirements.RequiresGPU == true && template.Name.ToLowerInvariant().Contains("enterprise"))
            score += 0.2m;

        // Match cost constraints
        if (requirements.CostConstraints?.MaxMonthlyCost.HasValue == true)
        {
            if (template.EstimatedMonthlyCost <= requirements.CostConstraints.MaxMonthlyCost)
                score += 0.2m;
            else
                score -= 0.1m;
        }

        return Math.Max(0, Math.Min(1, score));
    }

    private List<string> GenerateMatchReasons(LandingZoneTemplate template, LandingZoneRequirements requirements)
    {
        var reasons = new List<string>();

        if (requirements.RequiredServices.Intersect(template.RequiredFeatures).Any())
            reasons.Add($"Includes required services: {string.Join(", ", requirements.RequiredServices.Intersect(template.RequiredFeatures))}");

        if (requirements.CostConstraints?.MaxMonthlyCost >= template.EstimatedMonthlyCost)
            reasons.Add($"Fits within budget (${template.EstimatedMonthlyCost}/month)");

        if (template.Type == TemplateType.BasicAI && requirements.Environment == "dev")
            reasons.Add("Perfect for development environments");

        if (template.Type == TemplateType.EnterpriseAI && requirements.RequiresGPU == true)
            reasons.Add("Includes GPU compute for ML workloads");

        return reasons;
    }

    private Dictionary<string, object> GenerateRecommendedParameters(LandingZoneTemplate template, LandingZoneRequirements requirements)
    {
        var parameters = new Dictionary<string, object>();

        if (template.Parameters.ContainsKey("environment"))
            parameters["environment"] = requirements.Environment ?? "dev";

        if (template.Parameters.ContainsKey("nodeCount"))
        {
            var nodeCount = requirements.RequiresGPU == true ? 3 : 2;
            parameters["nodeCount"] = nodeCount;
        }

        if (template.Parameters.ContainsKey("enablePrivateEndpoints"))
            parameters["enablePrivateEndpoints"] = requirements.Environment == "prod";

        return parameters;
    }

    private CostEstimate CalculateCostEstimate(List<RecommendedTemplate> templates, LandingZoneRequirements requirements)
    {
        var primaryTemplate = templates.FirstOrDefault();
        if (primaryTemplate == null)
        {
            return new CostEstimate
            {
                EstimatedMonthlyCost = 500m,
                EstimatedDailyCost = 16.67m,
                CostBreakdown = new Dictionary<string, decimal>
                {
                    ["Base Infrastructure"] = 500m
                }
            };
        }

        var baseCost = primaryTemplate.Template.EstimatedMonthlyCost ?? 500m;
        
        // Adjust based on requirements
        if (requirements.RequiresGPU == true)
            baseCost *= 1.5m;
        
        if (requirements.Environment == "prod")
            baseCost *= 1.3m;

        return new CostEstimate
        {
            EstimatedMonthlyCost = baseCost,
            EstimatedDailyCost = baseCost / 30,
            CostBreakdown = new Dictionary<string, decimal>
            {
                ["Compute"] = baseCost * 0.6m,
                ["Storage"] = baseCost * 0.2m,
                ["AI Services"] = baseCost * 0.15m,
                ["Networking"] = baseCost * 0.05m
            },
            CostOptimizationSuggestions = new List<string>
            {
                "Consider using spot instances for development workloads",
                "Enable auto-scaling to optimize compute costs",
                "Use lifecycle policies for storage cost optimization"
            }
        };
    }

    private Dictionary<string, object> GenerateParameters(LandingZoneRequirements requirements)
    {
        return new Dictionary<string, object>
        {
            ["projectName"] = requirements.ProjectName ?? "ai-project",
            ["environment"] = requirements.Environment ?? "dev",
            ["enableGPU"] = requirements.RequiresGPU ?? false,
            ["estimatedCost"] = requirements.CostConstraints?.MaxMonthlyCost ?? 1000m
        };
    }

    private List<string> GenerateWarnings(LandingZoneRequirements requirements)
    {
        var warnings = new List<string>();

        if (requirements.CostConstraints?.MaxMonthlyCost < 300m)
            warnings.Add("Budget may be insufficient for production AI workloads");

        if (requirements.RequiresGPU == true && requirements.Environment == "dev")
            warnings.Add("GPU instances can be expensive for development - consider using CPU-only for initial development");

        if (string.IsNullOrEmpty(requirements.Environment))
            warnings.Add("Environment not specified - defaulting to development");

        return warnings;
    }

    private void InitializeDeploymentProgress(string landingZoneId)
    {
        var progress = new DeploymentProgress
        {
            LandingZoneId = landingZoneId,
            Status = LandingZoneStatus.Provisioning,
            ProgressPercentage = 5,
            CurrentStep = "Initializing Deployment",
            StartedAt = DateTime.UtcNow,
            EstimatedCompletionTime = DateTime.UtcNow.AddMinutes(15),
            Steps = new List<DeploymentStep>
            {
                new DeploymentStep { Name = "Validate Configuration", Status = StepStatus.Completed },
                new DeploymentStep { Name = "Create Resource Group", Status = StepStatus.InProgress },
                new DeploymentStep { Name = "Deploy Networking", Status = StepStatus.Pending },
                new DeploymentStep { Name = "Setup Storage", Status = StepStatus.Pending },
                new DeploymentStep { Name = "Configure AI Services", Status = StepStatus.Pending },
                new DeploymentStep { Name = "Apply Security Settings", Status = StepStatus.Pending },
                new DeploymentStep { Name = "Finalize Deployment", Status = StepStatus.Pending }
            }
        };

        _deploymentProgress[landingZoneId] = progress;
    }

    private static string GetRandomResponse(string category)
    {
        var responses = _aiResponses[category];
        var random = new Random();
        return responses[random.Next(responses.Count)];
    }
}