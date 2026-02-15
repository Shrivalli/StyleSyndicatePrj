using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Configuration;

namespace StyleSyndicatePrjBE.Agents;

/// <summary>
/// AutoGen-based Agent - Refactored from custom Agent base class
/// Each specialized agent extends this and provides custom system prompts and behaviors
/// Now supports Groq API for real LLM responses
/// </summary>
public abstract class AutoGenAgent
{
    protected GroqLLMProvider? _groqProvider;
    protected AutoGenConfig? _config;
    protected ILogger? _logger;

    public string AgentName { get; protected set; } = string.Empty;
    public string Role { get; protected set; } = string.Empty;
    public string SystemPrompt { get; protected set; } = string.Empty;
    
    protected List<AgentMessage> ConversationHistory { get; set; } = new();

    /// <summary>
    /// Initialize agent with Groq provider for real LLM calls
    /// </summary>
    public virtual void InitializeGroqProvider(GroqLLMProvider groqProvider, AutoGenConfig config, ILogger logger)
    {
        _groqProvider = groqProvider;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Generate a response for the given user input
    /// Uses Groq API if configured, otherwise falls back to mock response
    /// </summary>
    public virtual async Task<AgentMessage> ProcessAsync(string userInput)
    {
        string content;

        // Try to use Groq API if available and configured
        if (_groqProvider != null && _config?.IsConfigured() == true)
        {
            content = await _groqProvider.GenerateResponseAsync(SystemPrompt, userInput, ConversationHistory);
        }
        else
        {
            // Fall back to mock response
            content = GenerateResponse(userInput);
        }

        var message = new AgentMessage
        {
            Agent = AgentName,
            Role = Role,
            Content = content,
            Timestamp = DateTime.UtcNow
        };

        ConversationHistory.Add(message);
        return message;
    }

    /// <summary>
    /// Generate response based on agent role and system prompt
    /// Override in subclasses for custom logic (used as fallback)
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
