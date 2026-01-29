namespace StyleSyndicatePrjBE.Models;

public class StyleRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Occasion { get; set; } = string.Empty; // Wedding, Casual, Business, etc.
    public string Location { get; set; } = string.Empty; // Tuscany, etc.
    public DateTime EventDate { get; set; }
    public string WeatherForecast { get; set; } = string.Empty;
    public string UserPreferences { get; set; } = string.Empty;
    public int? CuratedOutfitId { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}

public class CuratedOutfit
{
    public int Id { get; set; }
    public int StyleRequestId { get; set; }
    public List<int> ProductIds { get; set; } = new();
    public List<string> Justifications { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public string CriticFeedback { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
