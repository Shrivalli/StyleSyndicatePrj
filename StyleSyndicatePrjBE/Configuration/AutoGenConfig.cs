using System.Text.Json;
using StyleSyndicatePrjBE.Models;

namespace StyleSyndicatePrjBE.Configuration;

/// <summary>
/// Groq Configuration - Handles LLM connection and model settings
/// Supports Groq API (OpenAI-compatible endpoint)
/// </summary>
public class AutoGenConfig
{
    public string? ApiKey { get; set; }
    public string? BaseUrl { get; set; } = "https://api.groq.com/openai/v1";
    public string? ModelName { get; set; }
    public int MaxTokens { get; set; } = 2000;
    public float Temperature { get; set; } = 0.7f;

    /// <summary>
    /// Validate Groq API configuration
    /// </summary>
    public bool IsConfigured()
    {
        return !string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(BaseUrl) && !string.IsNullOrEmpty(ModelName);
    }

    /// <summary>
    /// Get the full endpoint URL for Groq API
    /// </summary>
    public string GetChatCompletionEndpoint()
    {
        var baseUrl = BaseUrl?.TrimEnd('/') ?? "https://api.groq.com/openai/v1";
        return $"{baseUrl}/chat/completions";
    }

    // ChatCompletionOptions not available in AutoGen 0.4.0-dev.3
    // Commented out for compatibility
    /*
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
    */
}

/// <summary>
/// Groq LLM response generator - Uses Groq API for real LLM responses
/// </summary>
public class GroqLLMProvider
{
    private readonly AutoGenConfig _config;
    private readonly ILogger<GroqLLMProvider> _logger;

    public GroqLLMProvider(AutoGenConfig config, ILogger<GroqLLMProvider> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Generate response using Groq API via HTTP client
    /// </summary>
    public async Task<string> GenerateResponseAsync(string systemPrompt, string userMessage, List<AgentMessage> conversationHistory)
    {
        try
        {
            if (!_config.IsConfigured())
            {
                _logger.LogWarning("Groq API is not configured. Returning mock response.");
                return $"[Groq - Not Configured] {userMessage.Substring(0, Math.Min(50, userMessage.Length))}...";
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");

                var messages = new List<object> { new { role = "system", content = systemPrompt } };
                
                // Add conversation history
                foreach (var msg in conversationHistory)
                {
                    messages.Add(new { role = "user", content = msg.Content });
                }
                
                // Add current message
                messages.Add(new { role = "user", content = userMessage });

                var requestBody = new
                {
                    model = _config.ModelName,
                    messages = messages,
                    temperature = _config.Temperature,
                    max_tokens = _config.MaxTokens
                };

                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(requestBody),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(_config.GetChatCompletionEndpoint(), jsonContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Groq API error: {response.StatusCode} - {responseContent}");
                    return $"Error calling Groq API: {response.StatusCode}";
                }

                // Parse the response
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;
                if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var firstChoice = choices[0];
                    if (firstChoice.TryGetProperty("message", out var message) &&
                        message.TryGetProperty("content", out var content))
                    {
                        return content.GetString() ?? "No response from Groq";
                    }
                }

                return "Unable to parse Groq response";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calling Groq API: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}
