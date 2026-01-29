namespace StyleSyndicatePrjBE.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Size { get; set; } = "M"; // XS, S, M, L, XL, XXL
    public decimal Budget { get; set; }
    public string[] DislikedColors { get; set; } = Array.Empty<string>();
    public string[] PreferredBrands { get; set; } = Array.Empty<string>();
    public string[] AvoidedMaterials { get; set; } = Array.Empty<string>();
    public string FitPreference { get; set; } = "Regular"; // Slim, Regular, Loose
    public List<int> PastPurchaseIds { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
