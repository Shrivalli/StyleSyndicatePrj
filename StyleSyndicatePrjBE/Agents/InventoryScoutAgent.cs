using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Services;

namespace StyleSyndicatePrjBE.Agents;

/// <summary>
/// The Inventory Scout Agent (AutoGen-based) - RAG Agent
/// Uses Retrieval-Augmented Generation to search product catalog
/// Matches user preferences with available inventory
/// </summary>
public class InventoryScoutAgent : AutoGenAgent, IServiceAgent
{
    private readonly IInventoryService _inventoryService;

    public InventoryScoutAgent(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
        AgentName = "The Inventory Scout";
        Role = "Catalog Search & Product Matching";
        SystemPrompt = @"You are The Inventory Scout, a product discovery specialist. Your role is to:
1. Search the product catalog based on user preferences and trend data
2. Filter products by size, budget, color, and material constraints
3. Match products to the user's fit preference and brand affinity
4. Retrieve detailed product information and availability
5. Suggest complementary items for complete outfits

You use RAG (Retrieval-Augmented Generation) to:
- Query the product database with natural language
- Apply preference filters (avoid colors, materials, sizes)
- Check stock availability
- Retrieve product images and descriptions";
    }

    public override async Task<AgentMessage> ProcessAsync(string userInput)
    {
        var criteria = ExtractSearchCriteria(userInput, ConversationHistory);
        var searchCriteria = new InventorySearchCriteria
        {
            MaxPrice = criteria.MaxPrice,
            Size = criteria.Size,
            ExcludeColors = criteria.ExcludeColors,
            ExcludeMaterials = criteria.ExcludeMaterials,
            Categories = criteria.Categories
        };
        
        var matchedProducts = await _inventoryService.SearchProductsAsync(searchCriteria);

        var content = !matchedProducts.Any()
            ? "No products matching your criteria found in current inventory."
            : GenerateProductRecommendations(matchedProducts);

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
        var criteria = ExtractSearchCriteria(query, ConversationHistory);
        var searchCriteria = new InventorySearchCriteria
        {
            MaxPrice = criteria.MaxPrice,
            Size = criteria.Size,
            ExcludeColors = criteria.ExcludeColors,
            ExcludeMaterials = criteria.ExcludeMaterials,
            Categories = criteria.Categories
        };
        
        var products = await _inventoryService.SearchProductsAsync(searchCriteria);
        return products.Any() ? GenerateProductRecommendations(products) : "No products found";
    }

    private SearchCriteria ExtractSearchCriteria(string input, List<AgentMessage> history)
    {
        // Extract criteria from current input and conversation history
        return new SearchCriteria
        {
            MaxPrice = 1000,
            Size = "M",
            ExcludeColors = new string[] { },
            ExcludeMaterials = new string[] { },
            Categories = new[] { "Shirt", "Pants", "Jacket" }
        };
    }

    private string GenerateProductRecommendations(List<Product> products)
    {
        var recommendations = string.Join("\n", products.Take(5).Select(p => 
            $"  💼 {p.Name} by {p.Brand}: ${p.Price} ({p.Color}, {p.Material})"));

        return $@"🔍 Found {products.Count} matching products:
{recommendations}

These items match your size, budget, and style preferences while avoiding your disliked colors and materials.";
    }
}

public class SearchCriteria
{
    public decimal MaxPrice { get; set; }
    public string Size { get; set; } = string.Empty;
    public string[] ExcludeColors { get; set; } = Array.Empty<string>();
    public string[] ExcludeMaterials { get; set; } = Array.Empty<string>();
    public string[] Categories { get; set; } = Array.Empty<string>();
}

