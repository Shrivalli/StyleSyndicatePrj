using Microsoft.AutoGen.Agents;
using StyleSyndicatePrjBE.Models;

namespace StyleSyndicatePrjBE.Agents;

/// <summary>
/// AutoGen-based Agent - Refactored from custom Agent base class
/// Each specialized agent extends this and provides custom system prompts and behaviors
/// </summary>
public abstract class AutoGenAgent
{
    public string AgentName { get; protected set; } = string.Empty;
    public string Role { get; protected set; } = string.Empty;
    public string SystemPrompt { get; protected set; } = string.Empty;
    
    protected List<AgentMessage> ConversationHistory { get; set; } = new();

    /// <summary>
    /// Generate a response for the given user input
    /// In production, this would use the AutoGen ConversableAgent with LLM integration
    /// </summary>
    public virtual Task<AgentMessage> ProcessAsync(string userInput)
    {
        var message = new AgentMessage
        {
            Agent = AgentName,
            Role = Role,
            Content = GenerateResponse(userInput),
            Timestamp = DateTime.UtcNow
        };

        ConversationHistory.Add(message);
        return Task.FromResult(message);
    }

    /// <summary>
    /// Generate response based on agent role and system prompt
    /// Override in subclasses for custom logic
    /// </summary>
    protected abstract string GenerateResponse(string userInput);

    /// <summary>
    /// Get conversation context for multi-turn conversations
    /// </summary>
    public List<AgentMessage> GetConversationHistory() => new(ConversationHistory);

    /// <summary>
    /// Clear conversation history for new requests
    /// </summary>
    public void ClearHistory()
    {
        ConversationHistory.Clear();
    }
}

/// <summary>
/// Helper interface for agents that interact with external services
/// </summary>
public interface IServiceAgent
{
    Task<string> QueryServiceAsync(string query);
}
