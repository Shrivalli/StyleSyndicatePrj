using StyleSyndicatePrjBE.Models;

namespace StyleSyndicatePrjBE.Services;

public interface IInventoryService
{
    Task<List<Product>> SearchProductsAsync(InventorySearchCriteria criteria);
    Task<Product?> GetProductByIdAsync(int productId);
}

public class InventorySearchCriteria
{
    public decimal MaxPrice { get; set; } = decimal.MaxValue;
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? Material { get; set; }
    public string[] ExcludeColors { get; set; } = Array.Empty<string>();
    public string[] ExcludeMaterials { get; set; } = Array.Empty<string>();
    public string[] Categories { get; set; } = Array.Empty<string>();
    public string[] PreferredBrands { get; set; } = Array.Empty<string>();
}

public class MockInventoryService : IInventoryService
{
    private readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Tuscany Linen Shirt", Brand = "Giorgio Armani", Category = "Shirt", Material = "Linen", Color = "Cream", Price = 450, AvailableSizes = new[] { "S", "M", "L", "XL" }, Tags = new[] { "Trending", "Summer" }, InStock = true },
        new Product { Id = 2, Name = "Tailored Wool Blazer", Brand = "Gucci", Category = "Jacket", Material = "Wool", Color = "Navy", Price = 1200, AvailableSizes = new[] { "M", "L" }, Tags = new[] { "Formal", "Luxury" }, InStock = true },
        new Product { Id = 3, Name = "Earth Tone Chinos", Brand = "Prada", Category = "Pants", Material = "Cotton", Color = "Beige", Price = 350, AvailableSizes = new[] { "S", "M", "L" }, Tags = new[] { "Trending", "Versatile" }, InStock = true },
        new Product { Id = 4, Name = "Silk Pocket Square", Brand = "Hermès", Category = "Accessory", Material = "Silk", Color = "Terracotta", Price = 180, AvailableSizes = new[] { "One Size" }, Tags = new[] { "Luxury", "Accessory" }, InStock = true },
        new Product { Id = 5, Name = "Cashmere Blend Sweater", Brand = "Chanel", Category = "Top", Material = "Cashmere", Color = "Sage", Price = 800, AvailableSizes = new[] { "XS", "S", "M" }, Tags = new[] { "Luxury", "Cozy" }, InStock = true },
        new Product { Id = 6, Name = "Summer Linen Jacket", Brand = "Prada", Category = "Jacket", Material = "Linen", Color = "Cream", Price = 650, AvailableSizes = new[] { "M", "L", "XL" }, Tags = new[] { "Summer", "Trending" }, InStock = true },
        new Product { Id = 7, Name = "Luxury Oxford Shoes", Brand = "Gucci", Category = "Shoes", Material = "Leather", Color = "Brown", Price = 600, AvailableSizes = new[] { "8", "9", "10", "11", "12" }, Tags = new[] { "Formal", "Luxury" }, InStock = true },
        new Product { Id = 8, Name = "Terracotta Linen Pants", Brand = "Giorgio Armani", Category = "Pants", Material = "Linen", Color = "Terracotta", Price = 380, AvailableSizes = new[] { "S", "M", "L" }, Tags = new[] { "Summer", "Trending" }, InStock = true }
    };

    public Task<List<Product>> SearchProductsAsync(InventorySearchCriteria criteria)
    {
        var results = _products.Where(p =>
            p.Price <= criteria.MaxPrice &&
            (criteria.Size == null || p.AvailableSizes.Contains(criteria.Size)) &&
            (criteria.Color == null || p.Color.Equals(criteria.Color, StringComparison.OrdinalIgnoreCase)) &&
            (criteria.Material == null || p.Material.Equals(criteria.Material, StringComparison.OrdinalIgnoreCase)) &&
            !criteria.ExcludeColors.Contains(p.Color, StringComparer.OrdinalIgnoreCase) &&
            !criteria.ExcludeMaterials.Contains(p.Material, StringComparer.OrdinalIgnoreCase) &&
            (criteria.Categories.Length == 0 || criteria.Categories.Contains(p.Category)) &&
            p.InStock
        ).ToList();

        return Task.FromResult(results);
    }

    public Task<Product?> GetProductByIdAsync(int productId)
    {
        return Task.FromResult(_products.FirstOrDefault(p => p.Id == productId));
    }
}
