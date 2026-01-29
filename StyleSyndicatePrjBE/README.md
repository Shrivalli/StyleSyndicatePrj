# Style Syndicate API - Multi-Agent Fashion Orchestrator

## Overview
The Style Syndicate is an AI-powered multi-agent fashion orchestrator built with .NET Core. It uses autonomous agents working together in a GroupChat workflow to provide personalized fashion recommendations.

### Example Use Case
**User Request**: "I have a wedding in Tuscany next month, help me look sharp"

The system triggers a coordinated workflow where different agents play specific roles to curate the perfect outfit.

---

## Agent Architecture

### 1. **The Concierge Agent** (User Proxy)
**Role**: User Proxy & Presentation Manager

Interfaces directly with customers, asking clarifying questions about:
- Budget constraints
- Weather expectations  
- Fit preferences (Slim, Regular, Loose)
- Occasion and location details

Presents the final curated outfit with "Why we picked this" justifications.

**Endpoint**: Interacts through `/api/stylesyndicate/curate-outfit`

---

### 2. **The Historian Agent** (Data Agent)
**Role**: User Data & Preference Analyst

Retrieves comprehensive user profile data:
- Size and fit preferences
- Past purchases and brand affinity
- Disliked colors and materials
- Budget ranges

**Query Example**:
```
GET /api/users/{userId}
```

---

### 3. **The Trend Analyst Agent** (Web Agent)
**Role**: Fashion Trend & Weather Specialist

Analyzes:
- Fashion trends for specific locations and time periods
- Weather patterns and their fashion implications
- Cultural dress codes and expectations
- Seasonal style recommendations

**Mock Response**: Returns trending styles, recommended materials, and color palettes based on location and date.

---

### 4. **The Inventory Scout Agent** (RAG Agent)
**Role**: Catalog Search & Product Matching

Uses Retrieval-Augmented Generation to:
- Query the product catalog with natural language
- Filter products by size, budget, color, material constraints
- Match products to user's fit preference and brand affinity
- Retrieve detailed product information and availability

**Search Endpoint**:
```
POST /api/products/search
Body: {
  "maxPrice": 2000,
  "size": "L",
  "excludeColors": ["yellow"],
  "excludeMaterials": ["polyester"],
  "categories": ["Shirt", "Pants", "Jacket"]
}
```

---

### 5. **The Critic Agent** (QA Agent)
**Role**: Outfit Quality Assurance & Validation

Critically reviews proposed outfits:
- Validates material appropriateness for weather
- Checks color and style harmony
- Ensures budget compliance
- Validates brand alignment with preferences
- Ensures overall outfit coherence

Provides constructive feedback and requests revisions if needed.

---

## Technical Flow

### GroupChat Coordination Process

```
User Request (e.g., "Wedding in Tuscany next month")
    ↓
[Concierge] → Greets user, gathers requirements
    ↓
[Historian] → Looks up User #882: "Size L, slim fit, dislikes yellow"
    ↓
[Trend Analyst] → Reports: "Tuscany May = Linen blends, earth tones, breezy"
    ↓
[Inventory Scout] → Queries: SELECT * FROM products WHERE 
                    material='linen' AND color!='yellow' AND size='L'
    ↓
[Critic] → Reviews outfit: "Wait, wool blazer? Too hot! Find lighter option"
    ↓
[Concierge] → Presents final outfit with explanations
```

---

## Project Structure

```
StyleSyndicatePrjBE/
├── Models/
│   ├── User.cs                 # User profile model
│   ├── Product.cs              # Product catalog model
│   ├── StyleRequest.cs         # Style request and curated outfit models
│   └── AgentMessage.cs         # Agent conversation models
│
├── Services/
│   ├── IUserDataService.cs     # User data retrieval & storage
│   ├── ITrendService.cs        # Trend analysis service
│   ├── IInventoryService.cs    # Product catalog service
│   └── IGroupChatManager.cs    # Multi-agent orchestrator
│
├── Agents/
│   ├── Agent.cs                # Base agent abstract class
│   ├── ConciergeAgent.cs       # User-facing agent
│   ├── HistorianAgent.cs       # User data agent
│   ├── TrendAnalystAgent.cs    # Trend analysis agent
│   ├── InventoryScoutAgent.cs  # Catalog search agent
│   └── CriticAgent.cs          # Quality assurance agent
│
├── Controllers/
│   ├── StyleSyndicateController.cs  # Main orchestration endpoints
│   ├── UsersController.cs           # User management endpoints
│   └── ProductsController.cs        # Product search endpoints
│
├── Program.cs                  # Service registration & configuration
└── appsettings.json           # Configuration
```

---

## API Endpoints

### Style Curation Workflow
```
POST /api/stylesyndicate/curate-outfit?userId=1
Body: "I have a wedding in Tuscany next month, help me look sharp"

Response: {
  "requestId": 1,
  "messages": [
    {
      "agent": "The Concierge",
      "role": "User Proxy & Presentation Manager",
      "content": "...",
      "timestamp": "2026-01-29T10:30:00Z"
    },
    ...
  ],
  "finalOutfit": {
    "styleRequestId": 1,
    "productIds": [1, 3, 4, 7],
    "justifications": [
      "Linen shirt chosen for breathability in Tuscan heat",
      ...
    ],
    "totalPrice": 1430,
    "criticFeedback": "Outfit approved: cohesive, appropriate, and within budget"
  },
  "status": "Completed"
}
```

### User Management
```
GET    /api/users/{userId}           # Get user profile
POST   /api/users                    # Create/update user
```

### Product Catalog
```
GET    /api/products/{productId}                    # Get product details
POST   /api/products/search                        # Search products (POST)
GET    /api/products/search?maxPrice=2000&size=L  # Search products (GET)
```

---

## Swagger/OpenAPI Documentation

Access the interactive API documentation:
- **HTTP**: `http://localhost:5118/swagger/index.html`
- **HTTPS**: `https://localhost:7208/swagger/index.html`

---

## Mock Data

The system includes mock implementations for quick testing:

### Sample User (ID: 1)
- Name: John Doe
- Size: L (Slim fit)
- Budget: $2,000
- Disliked Colors: Yellow, Neon
- Preferred Brands: Gucci, Prada, Giorgio Armani

### Sample Products
- Tuscany Linen Shirt (Cream) - $450
- Tailored Wool Blazer (Navy) - $1,200
- Earth Tone Chinos (Beige) - $350
- Silk Pocket Square (Terracotta) - $180
- And more...

---

## How to Extend

### Adding a New Agent
1. Create a new class inheriting from `Agent`
2. Implement the `ProcessAsync` method
3. Register in `Program.cs` with `builder.Services.AddScoped<YourAgent>()`
4. Update `GroupChatManager` to include your agent in the workflow

### Connecting to Real Data
Replace mock services with actual implementations:
- `IUserDataService` → Connect to user database (SQL Server, MongoDB, etc.)
- `ITrendService` → Integrate with Bing/Google Search API
- `IInventoryService` → Connect to product database

### Integrating with OpenAI/Claude
Replace agent response generation with LLM calls:
```csharp
// In agent classes, replace mock responses with:
var response = await _openAiService.CompleteAsync(this.SystemPrompt, userInput);
```

---

## Technologies Used
- **.NET 9.0** - Web framework
- **C# 12** - Language
- **Swagger/OpenAPI** - API documentation
- **Dependency Injection** - Service registration
- **SOLID Principles** - Clean architecture

---

## Running the Project

```powershell
# Restore dependencies
dotnet restore

# Run with HTTPS
dotnet run --launch-profile https

# Run with HTTP
dotnet run --launch-profile http
```

Visit the Swagger UI at the URL provided by the launch profile.

---

## Example Workflow

### User Query
```
User: "I have a wedding in Tuscany next month, help me look sharp"
```

### Agent Conversation Flow
1. **Concierge**: "Thank you for reaching out! What's your budget and fit preference?"
2. **Historian**: "User prefers slim fit, size L, budget $2,000, dislikes yellow"
3. **Trend Analyst**: "Tuscany May weather: 25-30°C, trending styles: linen blends, earth tones"
4. **Inventory Scout**: Found 8 matching products in cream, beige, terracotta colors
5. **Critic**: "✓ Outfit approved - all materials suit weather, colors harmonize, within budget"
6. **Concierge**: Presents final outfit with styled explanations

### Result
A curated outfit with 4-5 complementary pieces totaling ~$1,430, with detailed justifications for each selection.

---

## License
For internal use within Style Syndicate organization.
