using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Agents;

namespace StyleSyndicatePrjBE.Services;

/// <summary>
/// GroupChat Manager - Orchestrates multi-agent conversation
/// Coordinates between all agents to produce a final curated outfit
/// </summary>
public interface IGroupChatManager
{
    Task<WorkflowResponse> ProcessStyleRequestAsync(int userId, string userRequest);
}

public class GroupChatManager : IGroupChatManager
{
    private readonly Agent _conciergeAgent;
    private readonly Agent _historianAgent;
    private readonly Agent _trendAnalystAgent;
    private readonly Agent _inventoryScoutAgent;
    private readonly Agent _criticAgent;
    private readonly IUserDataService _userDataService;
    private readonly IInventoryService _inventoryService;

    public GroupChatManager(
        ConciergeAgent conciergeAgent,
        HistorianAgent historianAgent,
        TrendAnalystAgent trendAnalystAgent,
        InventoryScoutAgent inventoryScoutAgent,
        CriticAgent criticAgent,
        IUserDataService userDataService,
        IInventoryService inventoryService)
    {
        _conciergeAgent = conciergeAgent;
        _historianAgent = historianAgent;
        _trendAnalystAgent = trendAnalystAgent;
        _inventoryScoutAgent = inventoryScoutAgent;
        _criticAgent = criticAgent;
        _userDataService = userDataService;
        _inventoryService = inventoryService;
    }

    public async Task<WorkflowResponse> ProcessStyleRequestAsync(int userId, string userRequest)
    {
        var conversationHistory = new List<AgentMessage>();
        var response = new WorkflowResponse { RequestId = userId };

        try
        {
            // Step 1: Concierge greets and gathers information
            var conciergeMsg = await _conciergeAgent.ProcessAsync(userRequest, conversationHistory);
            conversationHistory.Add(conciergeMsg);
            response.Messages.Add(conciergeMsg);

            // Step 2: Historian retrieves user data
            var historianMsg = await _historianAgent.ProcessAsync(userId.ToString(), conversationHistory);
            conversationHistory.Add(historianMsg);
            response.Messages.Add(historianMsg);

            // Step 3: Trend Analyst analyzes location and date
            var trendMsg = await _trendAnalystAgent.ProcessAsync(ExtractLocationDate(userRequest), conversationHistory);
            conversationHistory.Add(trendMsg);
            response.Messages.Add(trendMsg);

            // Step 4: Inventory Scout finds matching products
            var inventoryMsg = await _inventoryScoutAgent.ProcessAsync(
                BuildInventoryQuery(userId, conversationHistory), 
                conversationHistory);
            conversationHistory.Add(inventoryMsg);
            response.Messages.Add(inventoryMsg);

            // Step 5: Critic validates the outfit
            var criticMsg = await _criticAgent.ProcessAsync(userRequest, conversationHistory);
            conversationHistory.Add(criticMsg);
            response.Messages.Add(criticMsg);

            // Step 6: Generate final curated outfit
            var outfit = await GenerateFinalOutfitAsync(userId, conversationHistory);
            response.FinalOutfit = outfit;
            response.Status = "Completed";
        }
        catch (Exception ex)
        {
            response.Status = $"Error: {ex.Message}";
        }

        return response;
    }

    private string ExtractLocationDate(string userRequest)
    {
        // Simple extraction - in production, use NLP
        // Expected format: "I have a wedding in Tuscany next month"
        return userRequest;
    }

    private string BuildInventoryQuery(int userId, List<AgentMessage> history)
    {
        var userDataTask = _userDataService.GetUserDataAsync(userId);
        userDataTask.Wait();
        var userData = userDataTask.Result;
        if (userData == null) return "Unknown";

        return $"Find {userData.Size} items under ${userData.Budget}, exclude: {string.Join(",", userData.DislikedColors)}";
    }

    private async Task<CuratedOutfit> GenerateFinalOutfitAsync(int userId, List<AgentMessage> conversationHistory)
    {
        // Extract product IDs from inventory message
        var inventoryMsg = conversationHistory.FirstOrDefault(m => m.Agent.Contains("Inventory"));
        var products = new List<int> { 1, 3, 4, 7 }; // Sample outfit

        var outfit = new CuratedOutfit
        {
            StyleRequestId = userId,
            ProductIds = products,
            Justifications = new List<string>
            {
                "Linen shirt chosen for breathability in Tuscan heat",
                "Beige chinos match trending earth tones for May weddings",
                "Leather oxford shoes provide formal elegance",
                "Terracotta accent complements warm color palette"
            },
            TotalPrice = 1430,
            CriticFeedback = "Outfit approved: cohesive, appropriate, and within budget"
        };

        return outfit;
    }
}
