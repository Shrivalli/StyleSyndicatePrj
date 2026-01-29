namespace StyleSyndicatePrjBE.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Shirt, Pants, Jacket, etc.
    public string Material { get; set; } = string.Empty; // Cotton, Linen, Wool, etc.
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string[] AvailableSizes { get; set; } = Array.Empty<string>();
    public string Brand { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>(); // Trending, Casual, Formal, etc.
    public bool InStock { get; set; } = true;
    public string ImageUrl { get; set; } = string.Empty;
}
