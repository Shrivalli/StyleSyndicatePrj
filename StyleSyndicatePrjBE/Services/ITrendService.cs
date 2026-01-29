using StyleSyndicatePrjBE.Models;

namespace StyleSyndicatePrjBE.Services;

public class TrendData
{
    public string WeatherDescription { get; set; } = string.Empty;
    public string TemperatureRange { get; set; } = string.Empty;
    public string[] TrendingStyles { get; set; } = Array.Empty<string>();
    public string[] RecommendedMaterials { get; set; } = Array.Empty<string>();
    public string[] TrendingColors { get; set; } = Array.Empty<string>();
    public string StyleNotes { get; set; } = string.Empty;
}

public interface ITrendService
{
    Task<TrendData> GetTrendsAsync(string location, DateTime date);
}

public class MockTrendService : ITrendService
{
    public Task<TrendData> GetTrendsAsync(string location, DateTime date)
    {
        // Mock trend data based on location and date
        var isSummer = date.Month >= 5 && date.Month <= 9;
        
        var trends = new TrendData
        {
            WeatherDescription = isSummer ? "Warm and breezy" : "Cool and crisp",
            TemperatureRange = isSummer ? "25-30" : "10-18",
            TrendingStyles = isSummer 
                ? new[] { "Linen blends", "Light layers", "Loose cuts" }
                : new[] { "Structured blazers", "Wool blends", "Earth tones" },
            RecommendedMaterials = isSummer
                ? new[] { "Linen", "Cotton", "Silk" }
                : new[] { "Wool", "Cashmere", "Heavy cotton" },
            TrendingColors = isSummer
                ? new[] { "Cream", "Beige", "Terracotta", "Sage green" }
                : new[] { "Charcoal", "Navy", "Burgundy", "Olive" },
            StyleNotes = isSummer
                ? "Focus on breathability and movement. Neutral earth tones are dominating luxury fashion."
                : "Layering is key. Rich, warm tones convey elegance and sophistication."
        };

        return Task.FromResult(trends);
    }
}
