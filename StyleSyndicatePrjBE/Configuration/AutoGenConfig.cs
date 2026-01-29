using Azure.AI.OpenAI;
using Azure;

namespace StyleSyndicatePrjBE.Configuration;

/// <summary>
/// AutoGen Configuration - Handles LLM connection and model settings
/// </summary>
public class AutoGenConfig
{
    public string? ApiKey { get; set; }
    public string? Endpoint { get; set; }
    public string? ModelName { get; set; }
    public int MaxTokens { get; set; } = 2000;
    public float Temperature { get; set; } = 0.7f;

    /// <summary>
    /// Create Azure OpenAI client for use with AutoGen agents
    /// </summary>
    public AzureOpenAIClient CreateOpenAIClient()
    {
        if (string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(Endpoint))
        {
            throw new InvalidOperationException("Azure OpenAI credentials are not configured");
        }

        return new AzureOpenAIClient(
            new Uri(Endpoint),
            new AzureKeyCredential(ApiKey));
    }

    /// <summary>
    /// Create a mock client for development/testing (when Azure credentials unavailable)
    /// </summary>
    public static ChatCompletionOptions CreateDefaultChatOptions(int? maxTokens = null)
    {
        return new ChatCompletionOptions
        {
            MaxTokens = maxTokens ?? 2000,
            Temperature = 0.7f,
            TopP = 0.9f
        };
    }
}

/// <summary>
/// Mock LLM response generator for development (replaces real API calls)
/// </summary>
public class MockLLMProvider
{
    private readonly Random _random = new Random();
    
    public string GenerateResponse(string systemPrompt, string userMessage, List<string> conversationHistory)
    {
        // This is a mock - in production, use real LLM API
        return $"[Mock Response] Processing: {userMessage.Substring(0, Math.Min(30, userMessage.Length))}...";
    }

    public async Task<string> GenerateResponseAsync(string systemPrompt, string userMessage, List<string> conversationHistory)
    {
        // Simulate async LLM call
        await Task.Delay(100);
        return GenerateResponse(systemPrompt, userMessage, conversationHistory);
    }
}
