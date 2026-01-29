# Style Syndicate Architecture Diagram

## System Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                      API CLIENT (User/Frontend)                 │
│                                                                  │
│  "I have a wedding in Tuscany next month,                       │
│   help me look sharp"                                           │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         │ POST /api/stylesyndicate/curate-outfit
                         │
                    ┌────▼─────────────────────────────────────┐
                    │  StyleSyndicateController                │
                    │  - Validates request                     │
                    │  - Routes to GroupChatManager            │
                    └────┬──────────────────────────────────────┘
                         │
                         ▼
        ┌────────────────────────────────────────────┐
        │     IGroupChatManager (Orchestrator)       │
        │  ProcessStyleRequestAsync()                │
        │                                            │
        │  Coordinates multi-agent workflow          │
        └─┬──────┬──────┬──────┬──────────────────┬──┘
          │      │      │      │                  │
    Step 1│ Step2│ Step3│ Step4│ Step 5           │
          │      │      │      │                  │
    ┌─────▼──┐   │      │      │                  │
    │Concierge   │ │      │      │                  │
    │Agent    │   │      │      │                  │
    │         │   │      │      │                  │
    │ • Greet │   │      │      │                  │
    │ • Ask   │   │      │      │                  │
    │  budget │   │      │      │                  │
    │ • Gather    │      │      │                  │
    │  prefs  │   │      │      │                  │
    └─────────┘   │      │      │                  │
                  │      │      │                  │
          ┌───────▼──────┐      │                  │
          │ HistorianAgent│      │                  │
          │              │      │                  │
          │ • Query user │      │                  │
          │   database   │      │                  │
          │ • Extract    │      │                  │
          │   prefs      │      │                  │
          │ • Get past   │      │                  │
          │   purchases  │      │                  │
          └───────┬──────┘      │                  │
                  │ UserData    │                  │
          ┌───────▼──────┐      │                  │
          │IUserDataService      │                  │
          │(MockUserDataService) │                  │
          └────────────────────  │                  │
                              │      │                  │
                      ┌───────▼──────────┐          │
                      │ TrendAnalystAgent │          │
                      │                  │          │
                      │ • Get location   │          │
                      │ • Check weather  │          │
                      │ • Find trends    │          │
                      │ • Recommend      │          │
                      │   materials      │          │
                      └───────┬──────────┘          │
                              │ TrendData          │
                      ┌───────▼──────────┐          │
                      │ ITrendService    │          │
                      │(MockTrendService)│          │
                      └────────────────────          │
                                                ▼
                                    ┌───────────────────────┐
                                    │InventoryScoutAgent    │
                                    │                       │
                                    │ • Build search        │
                                    │   criteria            │
                                    │ • Query inventory     │
                                    │ • Filter products     │
                                    │ • Return matches      │
                                    └───────┬───────────────┘
                                            │ Products
                                    ┌───────▼───────────────┐
                                    │IInventoryService     │
                                    │(MockInventoryService)│
                                    │                      │
                                    │ 8 Sample Products:   │
                                    │ • Linen Shirts       │
                                    │ • Wool Blazers       │
                                    │ • Chinos             │
                                    │ • Accessories        │
                                    │ • Sweaters           │
                                    │ • Jackets            │
                                    │ • Shoes              │
                                    └──────────────────────┘
                                                │
                                    ┌───────────▼──────────┐
                                    │   CriticAgent        │
                                    │                      │
                                    │ • Validate outfit    │
                                    │ • Check materials    │
                                    │ • Verify budget      │
                                    │ • Confirm style      │
                                    │ • Provide feedback   │
                                    └───────┬──────────────┘
                                            │ Validation
                                    ┌───────▼──────────┐
                                    │ Final Outfit     │
                                    │ • ProductIds     │
                                    │ • Justifications │
                                    │ • Total Price    │
                                    │ • Critic Notes   │
                                    └───────┬──────────┘
                                            │
    ┌───────────────────────────────────────▼────────────────────┐
    │              WorkflowResponse (JSON)                       │
    │                                                            │
    │ {                                                         │
    │   "requestId": 1,                                        │
    │   "messages": [                                          │
    │     { Agent 1 response },                               │
    │     { Agent 2 response },                               │
    │     ...                                                  │
    │   ],                                                     │
    │   "finalOutfit": {                                       │
    │     "productIds": [...],                                │
    │     "justifications": [...],                            │
    │     "totalPrice": 1430.00,                              │
    │     "criticFeedback": "..."                             │
    │   },                                                     │
    │   "status": "Completed"                                 │
    │ }                                                        │
    └────────────────────────┬─────────────────────────────────┘
                             │
                             │ HTTP Response
                             │
                    ┌────────▼──────────┐
                    │   API Client       │
                    │   Receives Final   │
                    │   Curated Outfit   │
                    │   with Reasoning   │
                    └───────────────────┘
```

---

## Agent Sequence Flow

```
User Request
    │
    ├─► [1] CONCIERGE AGENT
    │       └─► "Thank you! What's your budget?"
    │
    ├─► [2] HISTORIAN AGENT
    │       └─► Queries: SELECT * FROM Users WHERE ID = 1
    │       └─► Returns: User preferences, size L, $2000 budget
    │
    ├─► [3] TREND ANALYST AGENT
    │       └─► Analyzes: Tuscany + May + Wedding
    │       └─► Returns: Linen, earth tones, breezy weather
    │
    ├─► [4] INVENTORY SCOUT AGENT
    │       └─► Builds Query: size=L, color!=yellow, material=linen
    │       └─► Returns: 8 matching products
    │
    ├─► [5] CRITIC AGENT
    │       └─► Validates: weather fit ✓, budget ✓, colors ✓
    │       └─► Approves outfit
    │
    └─► [6] CONCIERGE AGENT (Final)
            └─► Presents complete outfit with "why we picked this"
            └─► Returns to user with full justifications
```

---

## Service Dependencies

```
Controllers
├── StyleSyndicateController
│   └── IGroupChatManager
│       ├── ConciergeAgent
│       ├── HistorianAgent
│       │   └── IUserDataService (UserDataController)
│       ├── TrendAnalystAgent
│       │   └── ITrendService
│       ├── InventoryScoutAgent
│       │   └── IInventoryService (ProductsController)
│       └── CriticAgent
├── UsersController
│   └── IUserDataService
└── ProductsController
    └── IInventoryService
```

---

## Data Models Relationship

```
                    ┌─────────────┐
                    │ StyleRequest│
                    │             │
                    │ • UserId    │
                    │ • Occasion  │
                    │ • Location  │
                    │ • EventDate │
                    └────┬────────┘
                         │ Contains
                         │
                    ┌────▼──────────────┐
                    │ CuratedOutfit      │
                    │                    │
                    │ • ProductIds []    │
                    │ • Justifications[]  │
                    │ • TotalPrice       │
                    │ • CriticFeedback   │
                    └────┬───────────────┘
                         │ References
                         │
        ┌────────────────┴────────────────┐
        │                                  │
   ┌────▼──────────┐            ┌────────▼──────┐
   │ User          │            │ Product        │
   │               │            │                │
   │ • Id          │            │ • Id           │
   │ • Email       │            │ • Name         │
   │ • Name        │            │ • Category     │
   │ • Size        │            │ • Material     │
   │ • Budget      │            │ • Color        │
   │ • Preferences │            │ • Price        │
   │ • Brands      │            │ • Sizes []     │
   │ • Dislikes    │            │ • Brand        │
   │               │            │ • InStock      │
   └───────────────┘            └────────────────┘
```

---

## Agent Responsibility Matrix

```
┌──────────────────┬──────────┬─────────┬─────────┬──────────┬────────┐
│ Responsibility   │Concierge │Historian│Trend    │Inventory │Critic  │
│                  │          │         │Analyst  │Scout     │        │
├──────────────────┼──────────┼─────────┼─────────┼──────────┼────────┤
│ User Interface   │    ✓     │         │         │          │        │
│ Data Retrieval   │          │    ✓    │         │          │        │
│ Trend Analysis   │          │         │    ✓    │          │        │
│ Product Search   │          │         │         │    ✓     │        │
│ Validation       │          │         │         │          │   ✓    │
│ Presentation     │    ✓     │         │         │          │        │
│ Budget Check     │          │         │         │          │   ✓    │
│ Weather Fit      │          │         │    ✓    │          │   ✓    │
│ Color Harmony    │          │         │    ✓    │          │   ✓    │
│ Brand Alignment  │          │    ✓    │         │    ✓     │   ✓    │
└──────────────────┴──────────┴─────────┴─────────┴──────────┴────────┘
```

---

## API Endpoint Hierarchy

```
/api
├── /stylesyndicate
│   ├── POST /curate-outfit?userId={id}
│   │   ├── Input: User request string
│   │   └── Output: WorkflowResponse with messages and outfit
│   └── GET /workflow-history/{requestId}
│       └── Output: Stored workflow conversation
│
├── /users
│   ├── GET /{userId}
│   │   └── Output: User profile
│   └── POST /
│       ├── Input: User object
│       └── Output: Created user
│
└── /products
    ├── GET /{productId}
    │   └── Output: Product details
    ├── POST /search
    │   ├── Input: InventorySearchCriteria
    │   └── Output: Product[] matching criteria
    └── GET /search?maxPrice=X&size=Y&color=Z
        └── Output: Product[] matching query params
```

---

## Information Flow During Workflow

```
1. USER REQUEST
   ↓
   "I have a wedding in Tuscany next month, help me look sharp"
   
2. CONCIERGE AGENT
   ↓ (processes user input)
   Message: "Thank you! What's your budget and fit preference?"
   
3. HISTORIAN AGENT
   ↓ (queries database)
   UserData: {
     size: "L",
     budget: 2000,
     dislikedColors: ["yellow"],
     preferredBrands: ["Gucci", "Prada"]
   }
   
4. TREND ANALYST AGENT
   ↓ (analyzes location & date)
   TrendData: {
     weather: "Warm and breezy",
     materials: ["Linen", "Cotton", "Silk"],
     colors: ["Cream", "Beige", "Terracotta"]
   }
   
5. INVENTORY SCOUT AGENT
   ↓ (builds and executes search)
   SearchCriteria: {
     maxPrice: 2000,
     size: "L",
     excludeColors: ["yellow"],
     materials: ["Linen", "Cotton"],
     categories: ["Shirt", "Pants", "Jacket"]
   }
   ↓
   MatchedProducts: [
     {id: 1, name: "Tuscany Linen Shirt", ...},
     {id: 3, name: "Earth Tone Chinos", ...},
     {id: 4, name: "Silk Pocket Square", ...},
     {id: 7, name: "Luxury Oxford Shoes", ...}
   ]
   
6. CRITIC AGENT
   ↓ (validates selection)
   Validation: {
     ✓ All items within budget,
     ✓ Materials suit weather,
     ✓ Colors avoid dislikes,
     ✓ Brand preferences honored
   }
   
7. FINAL OUTFIT
   ↓ (presented by Concierge)
   CuratedOutfit: {
     productIds: [1, 3, 4, 7],
     justifications: [
       "Linen shirt for breathability",
       "Earth tones match trending colors",
       "Terracotta accessory adds warmth",
       "Oxford shoes provide formal touch"
     ],
     totalPrice: 1430.00
   }
   
8. RESPONSE TO USER
   ↓
   Complete WorkflowResponse with:
   - All agent messages (conversation history)
   - Final curated outfit
   - Detailed justifications
   - Total price and critic feedback
```

---

## Technology Stack Layers

```
┌───────────────────────────────────────────────────────────┐
│                    CLIENT LAYER                           │
│          (Browser, Mobile App, API Client)                │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│              PRESENTATION LAYER                         │
│   (Swagger UI, REST Endpoints, Response Formatting)     │
│                                                         │
│  • StyleSyndicateController                           │
│  • UsersController                                    │
│  • ProductsController                                 │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│              BUSINESS LOGIC LAYER                       │
│        (Agents, Orchestration, Workflow)               │
│                                                        │
│  • IGroupChatManager (Orchestrator)                   │
│  • ConciergeAgent                                     │
│  • HistorianAgent                                     │
│  • TrendAnalystAgent                                  │
│  • InventoryScoutAgent                                │
│  • CriticAgent                                        │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│              SERVICE LAYER                             │
│      (Data Access, External APIs, Utilities)           │
│                                                        │
│  • IUserDataService                                   │
│  • ITrendService                                      │
│  • IInventoryService                                  │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│              DATA LAYER                                │
│        (Databases, APIs, External Services)            │
│                                                        │
│  • User Database                                      │
│  • Product Inventory                                  │
│  • Trend Data (Weather, Fashion APIs)                 │
│  • LLM Services (Optional: OpenAI, Claude)            │
└───────────────────────────────────────────────────────────┘
```

---

## Deployment Architecture (Future)

```
┌─────────────────────────────────────────────────────────┐
│              AZURE / CLOUD DEPLOYMENT                  │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌─────────────────────────────────────────────────┐  │
│  │         Azure App Service (Web API)             │  │
│  │  • StyleSyndicatePrjBE .NET 9.0 Application    │  │
│  │  • Auto-scaling based on traffic               │  │
│  │  • HTTPS/TLS termination                       │  │
│  └──────────────────┬──────────────────────────────┘  │
│                     │                                  │
│  ┌──────────────────▼──────────────────────────────┐  │
│  │      Azure Database Solutions                  │  │
│  ├──────────────────────────────────────────────────┤  │
│  │ • Azure SQL Database (User Data)               │  │
│  │ • Cosmos DB (Product Catalog)                  │  │
│  │ • Redis Cache (Session/Trends)                 │  │
│  └──────────────────────────────────────────────────┘  │
│                                                         │
│  ┌──────────────────────────────────────────────────┐  │
│  │      External Service Integrations              │  │
│  ├──────────────────────────────────────────────────┤  │
│  │ • OpenAI API (LLM for Agent Responses)         │  │
│  │ • Bing Search API (Trend Analysis)             │  │
│  │ • Weather API (Climate Data)                   │  │
│  │ • SendGrid (Email Notifications)               │  │
│  └──────────────────────────────────────────────────┘  │
│                                                         │
│  ┌──────────────────────────────────────────────────┐  │
│  │      Monitoring & Analytics                     │  │
│  ├──────────────────────────────────────────────────┤  │
│  │ • Application Insights                         │  │
│  │ • Log Analytics                                │  │
│  │ • Performance Monitoring                       │  │
│  │ • Error Tracking                               │  │
│  └──────────────────────────────────────────────────┘  │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

**Diagram Version**: 1.0  
**Last Updated**: January 29, 2026  
**Architecture Pattern**: Agent-Based Multi-Service Architecture
