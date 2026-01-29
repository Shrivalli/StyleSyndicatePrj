using Microsoft.EntityFrameworkCore;
using StyleSyndicatePrjBE.Models;

namespace StyleSyndicatePrjBE.Data;

public class StyleSyndicateDbContext : DbContext
{
    public StyleSyndicateDbContext(DbContextOptions<StyleSyndicateDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<StyleRequest> StyleRequests { get; set; }
    public DbSet<CuratedOutfit> CuratedOutfits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed sample users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                Size = "L",
                Budget = 2000,
                DislikedColors = new[] { "yellow", "neon" },
                PreferredBrands = new[] { "Gucci", "Prada", "Giorgio Armani" },
                AvoidedMaterials = new[] { "polyester" },
                FitPreference = "Slim",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Email = "jane.smith@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                Size = "S",
                Budget = 1500,
                DislikedColors = new[] { "brown" },
                PreferredBrands = new[] { "Chanel", "Louis Vuitton" },
                AvoidedMaterials = new[] { "rough fabric" },
                FitPreference = "Regular",
                CreatedAt = DateTime.UtcNow
            }
        );

        // Seed sample products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Tuscany Linen Shirt", Brand = "Giorgio Armani", Category = "Shirt", Material = "Linen", Color = "Cream", Price = 450, AvailableSizes = new[] { "S", "M", "L", "XL" }, Tags = new[] { "Trending", "Summer" }, InStock = true },
            new Product { Id = 2, Name = "Tailored Wool Blazer", Brand = "Gucci", Category = "Jacket", Material = "Wool", Color = "Navy", Price = 1200, AvailableSizes = new[] { "M", "L" }, Tags = new[] { "Formal", "Luxury" }, InStock = true },
            new Product { Id = 3, Name = "Earth Tone Chinos", Brand = "Prada", Category = "Pants", Material = "Cotton", Color = "Beige", Price = 350, AvailableSizes = new[] { "S", "M", "L" }, Tags = new[] { "Trending", "Versatile" }, InStock = true },
            new Product { Id = 4, Name = "Silk Pocket Square", Brand = "Hermès", Category = "Accessory", Material = "Silk", Color = "Terracotta", Price = 180, AvailableSizes = new[] { "One Size" }, Tags = new[] { "Luxury", "Accessory" }, InStock = true },
            new Product { Id = 5, Name = "Cashmere Blend Sweater", Brand = "Chanel", Category = "Top", Material = "Cashmere", Color = "Sage", Price = 800, AvailableSizes = new[] { "XS", "S", "M" }, Tags = new[] { "Luxury", "Cozy" }, InStock = true },
            new Product { Id = 6, Name = "Summer Linen Jacket", Brand = "Prada", Category = "Jacket", Material = "Linen", Color = "Cream", Price = 650, AvailableSizes = new[] { "M", "L", "XL" }, Tags = new[] { "Summer", "Trending" }, InStock = true },
            new Product { Id = 7, Name = "Luxury Oxford Shoes", Brand = "Gucci", Category = "Shoes", Material = "Leather", Color = "Brown", Price = 600, AvailableSizes = new[] { "8", "9", "10", "11", "12" }, Tags = new[] { "Formal", "Luxury" }, InStock = true },
            new Product { Id = 8, Name = "Terracotta Linen Pants", Brand = "Giorgio Armani", Category = "Pants", Material = "Linen", Color = "Terracotta", Price = 380, AvailableSizes = new[] { "S", "M", "L" }, Tags = new[] { "Summer", "Trending" }, InStock = true }
        );
    }
}
