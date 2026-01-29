using StyleSyndicatePrjBE.Models;

namespace StyleSyndicatePrjBE.Agents;

public abstract class Agent
{
    public string Name { get; protected set; } = string.Empty;
    public string Role { get; protected set; } = string.Empty;
    public string SystemPrompt { get; protected set; } = string.Empty;
    
    public abstract Task<AgentMessage> ProcessAsync(string userInput, List<AgentMessage> conversationHistory);
}
