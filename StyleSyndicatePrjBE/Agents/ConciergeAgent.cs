using StyleSyndicatePrjBE.Models;

namespace StyleSyndicatePrjBE.Agents;

/// <summary>
/// The Concierge Agent (AutoGen-based)
/// Interfaces with the customer, gathers requirements, and presents curated outfits
/// </summary>
public class ConciergeAgent : AutoGenAgent
{
    public ConciergeAgent()
    {
        AgentName = "The Concierge";
        Role = "User Proxy & Presentation Manager";
        SystemPrompt = @"You are The Concierge, a luxury fashion stylist assistant. Your role is to:
1. Greet the customer warmly and gather essential information
2. Ask clarifying questions about their budget, weather expectations, fit preferences
3. Explain the styling process to the customer
4. Present the final curated outfit with 'Why we picked this' justifications
5. Be elegant, professional, and customer-focused

When gathering information, ask about:
- Budget constraints
- Occasion details
- Location and expected weather
- Personal fit preferences (Slim, Regular, Loose)
- Any colors or materials to avoid
- Preferred brands";
    }

    protected override string GenerateResponse(string userInput)
    {
        // In production with AutoGen, this would use the LLM
        // For now, provide intelligent defaults based on conversation pattern
        if (ConversationHistory.Count == 0)
        {
            return $"✨ Welcome to Style Syndicate! Thank you for reaching out! I'm your personal style concierge. " +
                   $"I can see you're looking for styling help with: '{userInput}'. " +
                   $"To create the perfect look, I'd love to know: " +
                   $"1) What's your budget range? 2) Do you prefer Slim, Regular, or Loose fitting clothes? " +
                   $"3) Are there any colors or materials you'd like to avoid?";
        }
        
        // For follow-up messages
        return $"Thank you for that information! Let me consult with my team of fashion experts " +
               $"to find the perfect outfit for you. I'll analyze trends, your preferences, and our inventory.";
    }
}
