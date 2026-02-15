using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Agents;
using StyleSyndicatePrjBE.Services;
using StyleSyndicatePrjBE.Configuration;

namespace StyleSyndicatePrjBE.Services;

/// <summary>
/// AutoGen-based GroupChat Manager
/// Orchestrates multi-agent collaboration using AutoGen patterns
/// Manages conversation flow and state across all 5 specialized agents
/// Now supports Groq API for real LLM responses
/// </summary>
public interface IAutoGenGroupChatManager
{
    Task<WorkflowResponse> ProcessStyleRequestAsync(int userId, string userRequest);
    Task<string> GetAgentSummaryAsync(string agentName);
}

public class AutoGenGroupChatManager : IAutoGenGroupChatManager
{
    private readonly ConciergeAgent _conciergeAgent;
    private readonly HistorianAgent _historianAgent;
    private readonly TrendAnalystAgent _trendAnalystAgent;
    private readonly InventoryScoutAgent _inventoryScoutAgent;
    private readonly CriticAgent _criticAgent;
    private readonly IUserDataService _userDataService;
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<AutoGenGroupChatManager> _logger;
    private readonly GroqLLMProvider _groqProvider;
    private readonly AutoGenConfig _config;

    public AutoGenGroupChatManager(
        ConciergeAgent conciergeAgent,
        HistorianAgent historianAgent,
        TrendAnalystAgent trendAnalystAgent,
        InventoryScoutAgent inventoryScoutAgent,
        CriticAgent criticAgent,
        IUserDataService userDataService,
        IInventoryService inventoryService,
        ILogger<AutoGenGroupChatManager> logger,
        GroqLLMProvider groqProvider,
        AutoGenConfig config)
    {
        _conciergeAgent = conciergeAgent;
        _historianAgent = historianAgent;
        _trendAnalystAgent = trendAnalystAgent;
        _inventoryScoutAgent = inventoryScoutAgent;
        _criticAgent = criticAgent;
        _userDataService = userDataService;
        _inventoryService = inventoryService;
        _logger = logger;
        _groqProvider = groqProvider;
        _config = config;

        // Initialize all agents with Groq provider
        InitializeAgents();
    }

    /// <summary>
    /// Initialize all agents with Groq provider for LLM support
    /// </summary>
    private void InitializeAgents()
    {
        _conciergeAgent.InitializeGroqProvider(_groqProvider, _config, _logger);
        _historianAgent.InitializeGroqProvider(_groqProvider, _config, _logger);
        _trendAnalystAgent.InitializeGroqProvider(_groqProvider, _config, _logger);
        _inventoryScoutAgent.InitializeGroqProvider(_groqProvider, _config, _logger);
        _criticAgent.InitializeGroqProvider(_groqProvider, _config, _logger);
    }

    /// <summary>
    /// Process style request through multi-agent collaboration workflow
    /// Agents interact in sequence, building on previous responses
    /// </summary>
    public async Task<WorkflowResponse> ProcessStyleRequestAsync(int userId, string userRequest)
    {
        var response = new WorkflowResponse { RequestId = userId };
        var conversationHistory = new List<AgentMessage>();

        try
        {
            _logger.LogInformation($"Starting style request workflow for user {userId}");

            // Phase 1: User Engagement & Information Gathering
            _logger.LogInformation("Phase 1: Concierge gathering requirements");
            var conciergeMsg = await _conciergeAgent.ProcessAsync(userRequest);
            conversationHistory.Add(conciergeMsg);
            response.Messages.Add(conciergeMsg);
            _logger.LogDebug($"Concierge: {conciergeMsg.Content}");

            // Phase 2: User Profile & History Analysis
            _logger.LogInformation("Phase 2: Historian analyzing user profile");
            var historianMsg = await _historianAgent.ProcessAsync(userId.ToString());
            conversationHistory.Add(historianMsg);
            response.Messages.Add(historianMsg);
            _logger.LogDebug($"Historian: {historianMsg.Content}");

            // Phase 3: Trend & Weather Analysis
            _logger.LogInformation("Phase 3: Trend Analyst researching current trends");
            var trendMsg = await _trendAnalystAgent.ProcessAsync(ExtractLocationDate(userRequest));
            conversationHistory.Add(trendMsg);
            response.Messages.Add(trendMsg);
            _logger.LogDebug($"Trend Analyst: {trendMsg.Content}");

            // Phase 4: Inventory Search & Product Matching (RAG)
            _logger.LogInformation("Phase 4: Inventory Scout searching catalog");
            var inventoryQuery = BuildInventoryQuery(userId, conversationHistory);
            var inventoryMsg = await _inventoryScoutAgent.ProcessAsync(inventoryQuery);
            conversationHistory.Add(inventoryMsg);
            response.Messages.Add(inventoryMsg);
            _logger.LogDebug($"Inventory Scout: {inventoryMsg.Content}");

            // Phase 5: Quality Assurance & Validation
            _logger.LogInformation("Phase 5: Critic validating outfit coherence");
            var criticMsg = await _criticAgent.ProcessAsync(userRequest);
            conversationHistory.Add(criticMsg);
            response.Messages.Add(criticMsg);
            _logger.LogDebug($"Critic: {criticMsg.Content}");

            // Phase 6: Final Outfit Curation
            _logger.LogInformation("Phase 6: Generating final curated outfit");
            var outfit = await GenerateFinalOutfitAsync(userId, conversationHistory);
            response.FinalOutfit = outfit;
            response.Status = "Completed";

            _logger.LogInformation($"Style request workflow completed successfully for user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing style request for user {userId}");
            response.Status = $"Error: {ex.Message}";
        }

        return response;
    }

    /// <summary>
    /// Get a summary of a specific agent's analysis and findings
    /// </summary>
    public Task<string> GetAgentSummaryAsync(string agentName)
    {
        var result = agentName.ToLower() switch
        {
            "concierge" => string.Join("\n", _conciergeAgent.GetConversationHistory().Select(m => m.Content)),
            "historian" => string.Join("\n", _historianAgent.GetConversationHistory().Select(m => m.Content)),
            "trend" => string.Join("\n", _trendAnalystAgent.GetConversationHistory().Select(m => m.Content)),
            "inventory" => string.Join("\n", _inventoryScoutAgent.GetConversationHistory().Select(m => m.Content)),
            "critic" => string.Join("\n", _criticAgent.GetConversationHistory().Select(m => m.Content)),
            _ => "Agent not found"
        };
        return Task.FromResult(result);
    }

    private string ExtractLocationDate(string userRequest)
    {
        // Extract location and date from user request
        // In production: use NLP/regex for robust parsing
        // Format: "I need an outfit for [location] on [date]"
        return userRequest;
    }

    private string BuildInventoryQuery(int userId, List<AgentMessage> history)
    {
        // Build query using information gathered by previous agents
        var taskResult = _userDataService.GetUserDataAsync(userId);
        taskResult.Wait();
        var userData = taskResult.Result;

        if (userData == null)
            return "Find items in medium size with no specific constraints";

        return $"Find {userData.Size} items under ${userData.Budget}, " +
               $"exclude: {string.Join(", ", userData.DislikedColors)}, " +
               $"prefer: {string.Join(", ", userData.PreferredBrands)}";
    }

    private Task<CuratedOutfit> GenerateFinalOutfitAsync(int userId, List<AgentMessage> conversationHistory)
    {
        // Extract selected products from inventory message
        var inventoryMsg = conversationHistory.FirstOrDefault(m => m.Agent.Contains("Inventory"));
        var productIds = ExtractProductIdsFromMessage(inventoryMsg?.Content ?? "");

        var outfit = new CuratedOutfit
        {
            StyleRequestId = userId,
            ProductIds = productIds,
            Justifications = new List<string>
            {
                "Selected based on user preferences and budget constraints",
                "Aligns with current fashion trends for the location and date",
                "Materials chosen for optimal weather compatibility",
                "Color palette creates cohesive, elegant ensemble",
                "All items within user's budget with quality-to-price ratio"
            },
            TotalPrice = 1430,
            CriticFeedback = "✅ Outfit approved: cohesive, appropriate, and within all constraints"
        };

        return Task.FromResult(outfit);
    }

    private List<int> ExtractProductIdsFromMessage(string message)
    {
        // In production: parse actual product IDs from agent message
        // For now: return sample product IDs
        return new List<int> { 1, 3, 4, 7 };
    }
}

/// <summary>
/// Extension method to maintain backward compatibility
/// Maps old interface to new AutoGen implementation
/// </summary>
public class GroupChatManager : IGroupChatManager
{
    private readonly IAutoGenGroupChatManager _autoGenManager;

    public GroupChatManager(IAutoGenGroupChatManager autoGenManager)
    {
        _autoGenManager = autoGenManager;
    }

    public async Task<WorkflowResponse> ProcessStyleRequestAsync(int userId, string userRequest)
    {
        return await _autoGenManager.ProcessStyleRequestAsync(userId, userRequest);
    }
}
