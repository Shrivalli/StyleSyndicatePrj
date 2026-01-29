using Microsoft.AspNetCore.Mvc;
using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Services;

namespace StyleSyndicatePrjBE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IInventoryService inventoryService, ILogger<ProductsController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    /// <summary>
    /// Search products by various criteria
    /// </summary>
    [HttpPost("search")]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchProducts([FromBody] InventorySearchCriteria criteria)
    {
        var products = await _inventoryService.SearchProductsAsync(criteria);
        return Ok(products);
    }

    /// <summary>
    /// Get product details by ID
    /// </summary>
    [HttpGet("{productId}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(int productId)
    {
        var product = await _inventoryService.GetProductByIdAsync(productId);
        
        if (product == null)
        {
            return NotFound($"Product {productId} not found");
        }

        return Ok(product);
    }

    /// <summary>
    /// Search products with query parameters (alternative to POST)
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchProductsQuery(
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? size,
        [FromQuery] string? color,
        [FromQuery] string? material,
        [FromQuery] string[] excludeColors,
        [FromQuery] string[] excludeMaterials,
        [FromQuery] string[] categories)
    {
        var criteria = new InventorySearchCriteria
        {
            MaxPrice = maxPrice ?? decimal.MaxValue,
            Size = size,
            Color = color,
            Material = material,
            ExcludeColors = excludeColors ?? Array.Empty<string>(),
            ExcludeMaterials = excludeMaterials ?? Array.Empty<string>(),
            Categories = categories ?? Array.Empty<string>()
        };

        var products = await _inventoryService.SearchProductsAsync(criteria);
        return Ok(products);
    }
}
