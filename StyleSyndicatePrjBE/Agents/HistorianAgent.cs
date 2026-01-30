using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Services;

namespace StyleSyndicatePrjBE.Agents;

/// <summary>
/// The Historian Agent (AutoGen-based) - Data Agent
/// Queries the user database for profile data
/// Retrieves user's size, past purchases, and preferred brands
/// </summary>
public class HistorianAgent : AutoGenAgent, IServiceAgent
{
    private readonly IUserDataService _userDataService;

    public HistorianAgent(IUserDataService userDataService)
    {
        _userDataService = userDataService;
        AgentName = "The Historian";
        Role = "User Data & Preference Analyst";
        SystemPrompt = @"You are The Historian, a data analyst agent specializing in customer profiles. Your role is to:
1. Query and analyze user profile data
2. Extract sizing information, past purchase history
3. Identify brand preferences and most-kept items
4. Summarize customer style patterns
5. Communicate findings clearly to other agents

You have access to customer databases and can retrieve:
- User size and fit preferences
- Purchase history and brand affinity
- Color and material preferences
- Budget ranges";
    }

    public override async Task<AgentMessage> ProcessAsync(string userInput)
    {
        // Extract user ID from input (in a real scenario, this would be parsed from the request)
        var userId = ExtractUserIdFromInput(userInput);
        
        if (userId == 0)
        {
            var errorMsg = new AgentMessage
            {
                Agent = AgentName,
                Role = Role,
                Content = "Unable to identify user. Please provide a valid user ID.",
                Timestamp = DateTime.UtcNow
            };
            ConversationHistory.Add(errorMsg);
            return errorMsg;
        }

        var userData = await _userDataService.GetUserDataAsync(userId);
        var content = GenerateHistorianReport(userData);
        
        var response = new AgentMessage
        {
            Agent = AgentName,
            Role = Role,
            Content = content,
            Timestamp = DateTime.UtcNow
        };

        ConversationHistory.Add(response);
        return response;
    }

    public async Task<string> QueryServiceAsync(string query)
    {
        if (int.TryParse(query, out var userId))
        {
            var user = await _userDataService.GetUserDataAsync(userId);
            return GenerateHistorianReport(user);
        }
        return "Unable to process query";
    }

    private int ExtractUserIdFromInput(string input)
    {
        // Simple extraction - in production, use more robust parsing
        if (int.TryParse(input, out var userId))
            return userId;
        
        return 0;
    }

    protected override string GenerateResponse(string userInput)
    {
        var userId = ExtractUserIdFromInput(userInput);
        if (userId == 0)
            return "Unable to identify user. Please provide a valid user ID.";
        return $"User profile retrieved for ID: {userId}";
    }

    private string GenerateHistorianReport(User? user)
    {
        if (user == null)
            return "No user data found.";

        var dislikedColors = string.Join(", ", user.DislikedColors.Length > 0 ? user.DislikedColors : new[] { "none" });
        var preferredBrands = string.Join(", ", user.PreferredBrands.Length > 0 ? user.PreferredBrands : new[] { "varied" });

        return $@"📊 User Profile Analysis:
- Name: {user.FirstName} {user.LastName}
- Size: {user.Size} ({user.FitPreference} fit preferred)
- Budget Range: ${user.Budget:F2}
- Disliked Colors: {dislikedColors}
- Preferred Brands: {preferredBrands}
- Past Purchases: {user.PastPurchaseIds.Count} items
- Member Since: {user.CreatedAt:MMMM yyyy}";
    }
}
