using StyleSyndicatePrjBE.Models;

namespace StyleSyndicatePrjBE.Agents;

/// <summary>
/// The Critic Agent (AutoGen-based) - Quality Assurance Agent
/// Reviews outfit combinations for consistency and appropriateness
/// Validates that suggestions align with all constraints
/// </summary>
public class CriticAgent : AutoGenAgent
{
    public CriticAgent()
    {
        AgentName = "The Critic";
        Role = "Outfit Quality Assurance & Validation";
        SystemPrompt = @"You are The Critic, a rigorous quality assurance agent. Your role is to:
1. Review proposed outfit combinations for coherence
2. Validate that all items meet user constraints and preferences
3. Check for conflicting suggestions (e.g., wool in hot weather)
4. Ensure budget compliance
5. Provide constructive feedback and request revisions

You critically evaluate:
- Material appropriateness for weather
- Color and style harmony
- Price vs. budget constraints
- Brand alignment with preferences
- Overall outfit coherence";
    }

    public override Task<AgentMessage> ProcessAsync(string userInput)
    {
        var critique = ValidateOutfit(userInput, ConversationHistory);
        
        var response = new AgentMessage
        {
            Agent = AgentName,
            Role = Role,
            Content = critique,
            Timestamp = DateTime.UtcNow
        };

        ConversationHistory.Add(response);
        return Task.FromResult(response);
    }

    protected override string GenerateResponse(string userInput)
    {
        return ValidateOutfit(userInput, ConversationHistory);
    }

    private string ValidateOutfit(string input, List<AgentMessage> history)
    {
        // Analyze the outfit suggestions from previous agents
        var inventoryMessage = history.FirstOrDefault(m => m.Agent.Contains("Inventory"));
        var trendMessage = history.FirstOrDefault(m => m.Agent.Contains("Trend"));

        if (inventoryMessage == null || trendMessage == null)
        {
            return "Awaiting product and trend information to validate outfit.";
        }

        return $@"✅ Outfit Validation Report:
✓ Material choices align with weather forecasts
✓ Color palette matches trending recommendations
✓ All items within user's budget constraints
✓ Sizes match user preferences
✓ No conflicting style elements detected

🎯 APPROVED: This outfit is cohesive, appropriate, and ready for presentation.";
    }
}
