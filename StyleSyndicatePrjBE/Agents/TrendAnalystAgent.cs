using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Services;

namespace StyleSyndicatePrjBE.Agents;

/// <summary>
/// The Trend Analyst Agent (AutoGen-based) - Web Agent
/// Searches for trending styles in specific locations and times
/// Provides context about weather, culture, and fashion trends
/// </summary>
public class TrendAnalystAgent : AutoGenAgent, IServiceAgent
{
    private readonly ITrendService _trendService;

    public TrendAnalystAgent(ITrendService trendService)
    {
        _trendService = trendService;
        AgentName = "The Trend Analyst";
        Role = "Fashion Trend & Weather Specialist";
        SystemPrompt = @"You are The Trend Analyst, a fashion forecasting expert. Your role is to:
1. Analyze fashion trends for specific locations and time periods
2. Research weather patterns and their fashion implications
3. Identify cultural style norms for the occasion and location
4. Recommend materials, colors, and styles that are trending
5. Provide insights on what people are wearing in target locations

You analyze:
- Current fashion trends
- Weather forecasts for the event location and date
- Cultural dress codes and expectations
- Seasonal style recommendations";
    }

    public override async Task<AgentMessage> ProcessAsync(string userInput)
    {
        var (location, date) = ExtractLocationAndDate(userInput);
        
        if (string.IsNullOrEmpty(location))
        {
            var errorMsg = new AgentMessage
            {
                Agent = AgentName,
                Role = Role,
                Content = "Please provide location and date information for trend analysis.",
                Timestamp = DateTime.UtcNow
            };
            ConversationHistory.Add(errorMsg);
            return errorMsg;
        }

        var trends = await _trendService.GetTrendsAsync(location, date);
        var content = GenerateTrendAnalysis(location, date, trends);
        
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
        var (location, date) = ExtractLocationAndDate(query);
        if (string.IsNullOrEmpty(location))
            return "Invalid location format";
            
        var trends = await _trendService.GetTrendsAsync(location, date);
        return GenerateTrendAnalysis(location, date, trends);
    }

    private (string location, DateTime date) ExtractLocationAndDate(string input)
    {
        // Simple extraction - in production, use NLP
        var parts = input.Split(',');
        var location = parts.Length > 0 ? parts[0].Trim() : string.Empty;
        var date = DateTime.Now.AddMonths(1);
        
        if (parts.Length > 1 && DateTime.TryParse(parts[1].Trim(), out var parsedDate))
            date = parsedDate;

        return (location, date);
    }

    protected override string GenerateResponse(string userInput)
    {
        var (location, date) = ExtractLocationAndDate(userInput);
        if (string.IsNullOrEmpty(location))
            return "Please provide location and date information for trend analysis.";
        return $"Trend analysis for {location} on {date:MMMM yyyy}";
    }

    private string GenerateTrendAnalysis(string location, DateTime date, TrendData trends)
    {
        return $@"🌍 Fashion & Weather Analysis for {location}:
- Event Date: {date:MMMM yyyy}
- Weather: {trends.WeatherDescription}, {trends.TemperatureRange}°C
- Top Trends: {string.Join(", ", trends.TrendingStyles)}
- Recommended Colors: {string.Join(", ", trends.TrendingColors)}
- Popular Materials: {string.Join(", ", trends.RecommendedMaterials)}
- Style Notes: {trends.StyleNotes}";
    }
}
