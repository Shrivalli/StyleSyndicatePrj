using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Data;
using Microsoft.EntityFrameworkCore;

namespace StyleSyndicatePrjBE.Services;

public class EFInventoryService : IInventoryService
{
    private readonly StyleSyndicateDbContext _context;

    public EFInventoryService(StyleSyndicateDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> SearchProductsAsync(InventorySearchCriteria criteria)
    {
        var query = _context.Products.AsQueryable();

        if (criteria.MaxPrice > 0)
            query = query.Where(p => p.Price <= criteria.MaxPrice);

        if (!string.IsNullOrEmpty(criteria.Size))
            query = query.Where(p => p.AvailableSizes.Contains(criteria.Size));

        if (!string.IsNullOrEmpty(criteria.Color))
            query = query.Where(p => p.Color.Equals(criteria.Color, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(criteria.Material))
            query = query.Where(p => p.Material.Equals(criteria.Material, StringComparison.OrdinalIgnoreCase));

        if (criteria.ExcludeColors.Length > 0)
            query = query.Where(p => !criteria.ExcludeColors.Contains(p.Color, StringComparer.OrdinalIgnoreCase));

        if (criteria.ExcludeMaterials.Length > 0)
            query = query.Where(p => !criteria.ExcludeMaterials.Contains(p.Material, StringComparer.OrdinalIgnoreCase));

        if (criteria.Categories.Length > 0)
            query = query.Where(p => criteria.Categories.Contains(p.Category));

        query = query.Where(p => p.InStock);

        return await query.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<bool> SaveProductAsync(Product product)
    {
        try
        {
            if (product.Id == 0)
            {
                _context.Products.Add(product);
            }
            else
            {
                var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(product);
                }
                else
                {
                    _context.Products.Add(product);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
