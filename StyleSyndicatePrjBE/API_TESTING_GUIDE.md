# API Testing Guide - Style Syndicate

## Quick Test Commands

### 1. Test the Full Workflow (Main Endpoint)

**Scenario**: User requests styling for a Tuscany wedding

```bash
curl -X POST "https://localhost:7208/api/stylesyndicate/curate-outfit?userId=1" \
  -H "Content-Type: application/json" \
  -d "\"I have a wedding in Tuscany next month, help me look sharp\"" \
  -k
```

**Expected Response**:
```json
{
  "requestId": 1,
  "messages": [
    {
      "agent": "The Concierge",
      "role": "User Proxy & Presentation Manager",
      "content": "[The Concierge] Thank you for reaching out! I understand you need styling help. To create the perfect look, I'd like to know: What is your budget range, and do you have any specific fit preferences?",
      "timestamp": "2026-01-29T10:30:00Z"
    },
    {
      "agent": "The Historian",
      "role": "User Data & Preference Analyst",
      "content": "[The Historian] User Profile Analysis:\n- Name: John Doe\n- Size: L (Slim fit preferred)\n- Budget Range: $2000.00\n- Disliked Colors: yellow, neon\n- Preferred Brands: Gucci, Prada, Giorgio Armani\n- Past Purchases: 0 items\n- Member Since: January 2026",
      "timestamp": "2026-01-29T10:30:01Z"
    },
    {
      "agent": "The Trend Analyst",
      "role": "Fashion Trend & Weather Specialist",
      "content": "[The Trend Analyst] Fashion & Weather Analysis for ...",
      "timestamp": "2026-01-29T10:30:02Z"
    },
    {
      "agent": "The Inventory Scout",
      "role": "Catalog Search & Product Matching",
      "content": "[The Inventory Scout] Found 8 matching products...",
      "timestamp": "2026-01-29T10:30:03Z"
    },
    {
      "agent": "The Critic",
      "role": "Outfit Quality Assurance & Validation",
      "content": "[The Critic] Outfit Validation:\n✓ Material choices align with weather forecasts\n...",
      "timestamp": "2026-01-29T10:30:04Z"
    }
  ],
  "finalOutfit": {
    "styleRequestId": 1,
    "productIds": [1, 3, 4, 7],
    "justifications": [
      "Linen shirt chosen for breathability in Tuscan heat",
      "Beige chinos match trending earth tones for May weddings",
      "Leather oxford shoes provide formal elegance",
      "Terracotta accent complements warm color palette"
    ],
    "totalPrice": 1430.00,
    "criticFeedback": "Outfit approved: cohesive, appropriate, and within budget"
  },
  "status": "Completed"
}
```

---

### 2. Get User Profile

**Scenario**: Retrieve details for user ID 1

```bash
curl -X GET "https://localhost:7208/api/users/1" \
  -H "Accept: application/json" \
  -k
```

**Expected Response**:
```json
{
  "id": 1,
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "size": "L",
  "budget": 2000.00,
  "dislikedColors": ["yellow", "neon"],
  "preferredBrands": ["Gucci", "Prada", "Giorgio Armani"],
  "avoidedMaterials": ["polyester"],
  "fitPreference": "Slim",
  "pastPurchaseIds": [],
  "createdAt": "2026-01-29T00:00:00Z"
}
```

---

### 3. Create/Update User Profile

**Scenario**: Register a new user

```bash
curl -X POST "https://localhost:7208/api/users" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 3,
    "email": "sarah.wilson@example.com",
    "firstName": "Sarah",
    "lastName": "Wilson",
    "size": "S",
    "budget": 1500.00,
    "dislikedColors": ["purple"],
    "preferredBrands": ["Chanel", "Louis Vuitton"],
    "avoidedMaterials": ["rough fabric"],
    "fitPreference": "Regular",
    "pastPurchaseIds": []
  }' \
  -k
```

---

### 4. Get Product by ID

**Scenario**: View details for Tuscany Linen Shirt (ID: 1)

```bash
curl -X GET "https://localhost:7208/api/products/1" \
  -H "Accept: application/json" \
  -k
```

**Expected Response**:
```json
{
  "id": 1,
  "name": "Tuscany Linen Shirt",
  "description": null,
  "category": "Shirt",
  "material": "Linen",
  "color": "Cream",
  "price": 450.00,
  "availableSizes": ["S", "M", "L", "XL"],
  "brand": "Giorgio Armani",
  "tags": ["Trending", "Summer"],
  "inStock": true,
  "imageUrl": ""
}
```

---

### 5. Search Products - Query Parameters

**Scenario**: Find all linen items under $500 in size L

```bash
curl -X GET "https://localhost:7208/api/products/search?maxPrice=500&size=L&material=Linen" \
  -H "Accept: application/json" \
  -k
```

---

### 6. Search Products - JSON Body (POST)

**Scenario**: Find blazers under $1500, avoid yellow and neon colors

```bash
curl -X POST "https://localhost:7208/api/products/search" \
  -H "Content-Type: application/json" \
  -d '{
    "maxPrice": 1500.00,
    "size": "L",
    "excludeColors": ["yellow", "neon"],
    "excludeMaterials": [],
    "categories": ["Jacket"],
    "preferredBrands": ["Gucci", "Prada"]
  }' \
  -k
```

**Expected Response**:
```json
[
  {
    "id": 2,
    "name": "Tailored Wool Blazer",
    "description": null,
    "category": "Jacket",
    "material": "Wool",
    "color": "Navy",
    "price": 1200.00,
    "availableSizes": ["M", "L"],
    "brand": "Gucci",
    "tags": ["Formal", "Luxury"],
    "inStock": true,
    "imageUrl": ""
  },
  {
    "id": 6,
    "name": "Summer Linen Jacket",
    "description": null,
    "category": "Jacket",
    "material": "Linen",
    "color": "Cream",
    "price": 650.00,
    "availableSizes": ["M", "L", "XL"],
    "brand": "Prada",
    "tags": ["Summer", "Trending"],
    "inStock": true,
    "imageUrl": ""
  }
]
```

---

## Testing with PowerShell

### Complete Workflow Test

```powershell
$uri = "https://localhost:7208/api/stylesyndicate/curate-outfit?userId=1"
$body = @"
"I have a wedding in Tuscany next month, help me look sharp"
"@

$response = Invoke-RestMethod -Uri $uri `
  -Method POST `
  -ContentType "application/json" `
  -Body $body `
  -SkipCertificateCheck

# Pretty print the response
$response | ConvertTo-Json -Depth 10 | Write-Host
```

### Get User Profile

```powershell
$response = Invoke-RestMethod -Uri "https://localhost:7208/api/users/1" `
  -Method GET `
  -SkipCertificateCheck

$response | ConvertTo-Json | Write-Host
```

### Search Products

```powershell
$searchBody = @{
    maxPrice = 2000
    size = "L"
    excludeColors = @("yellow")
    categories = @("Shirt", "Jacket", "Pants")
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://localhost:7208/api/products/search" `
  -Method POST `
  -ContentType "application/json" `
  -Body $searchBody `
  -SkipCertificateCheck

$response | ConvertTo-Json -Depth 5 | Write-Host
```

---

## Testing in Swagger UI

1. **Open Swagger**: Navigate to `https://localhost:7208/swagger/index.html`

2. **Try the Main Workflow**:
   - Click on `POST /api/stylesyndicate/curate-outfit`
   - Click "Try it out"
   - Enter `userId = 1`
   - Enter request text in the body field
   - Click "Execute"

3. **Explore Other Endpoints**:
   - `GET /api/users/{userId}` - Fetch user profile
   - `POST /api/products/search` - Search products
   - `GET /api/products/{productId}` - Get product details

---

## Sample Workflow Scenarios

### Scenario 1: Summer Wedding in Italy
```
Request: "I have a wedding in Tuscany next month, help me look sharp"
Expected: Linen shirt, light colors, earth tones
Budget: $2000
```

### Scenario 2: Business Conference
```
Request: "I need business formal attire for a tech conference in San Francisco next week"
Expected: Blazer, dress pants, structured looks
```

### Scenario 3: Casual Date Night
```
Request: "I'm going on a casual date to a restaurant, what should I wear"
Expected: Casual but stylish, relaxed fit, comfortable
```

---

## Troubleshooting

### Certificate Issues
If you get SSL certificate errors, add `-k` flag to curl or `-SkipCertificateCheck` to PowerShell

### Port Not Accessible
Ensure the application is running:
```bash
netstat -ano | findstr :7208
```

### 404 Not Found
- Check the exact endpoint path
- Verify user ID or product ID exists
- Use Swagger UI to see exact endpoints

### 500 Internal Server Error
- Check application logs in console
- Verify request body JSON format
- Ensure required fields are provided

---

## Performance Testing

For load testing, use tools like:
- **Apache JMeter**
- **Locust**
- **k6 by Grafana**

Example k6 script:
```javascript
import http from 'k6/http';
import { check } from 'k6';

export let options = {
  vus: 10,
  duration: '30s',
};

export default function () {
  let response = http.post(
    'https://localhost:7208/api/stylesyndicate/curate-outfit?userId=1',
    '"I have a wedding in Tuscany next month, help me look sharp"',
    {
      headers: {
        'Content-Type': 'application/json',
        'Insecure': true,
      },
    }
  );

  check(response, {
    'status is 200': (r) => r.status === 200,
    'response time < 1000ms': (r) => r.timings.duration < 1000,
  });
}
```

---

## Additional Resources

- [Swagger UI Documentation](https://swagger.io/tools/swagger-ui/)
- [OpenAPI Specification](https://spec.openapis.org/oas/v3.0.3)
- [.NET HTTP Client Documentation](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient)
- [cURL Documentation](https://curl.se/docs/)

---

**Last Updated**: January 29, 2026  
**API Version**: 1.0
