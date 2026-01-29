# Style Syndicate - Angular Client App

Angular-based frontend for the Style Syndicate multi-agent fashion consultation API.

## Project Structure

```
ClientApp/
├── src/
│   ├── app/
│   │   ├── components/
│   │   │   ├── style-request.component.ts       # Main style request component
│   │   │   ├── style-request.component.html     # Template
│   │   │   └── style-request.component.css      # Styles
│   │   ├── services/
│   │   │   └── api.service.ts                   # API communication service
│   │   ├── app.component.ts                     # Root component
│   │   ├── app.component.html
│   │   └── app.component.css
│   ├── main.ts                                   # Application bootstrap
│   ├── index.html                               # HTML template
│   ├── styles.css                               # Global styles
│   └── assets/                                   # Static assets
├── angular.json                                  # Angular CLI configuration
├── tsconfig.json                                # TypeScript configuration
├── tsconfig.app.json                           # App-specific TS config
├── package.json                                 # Dependencies
└── README.md                                    # This file
```

## Features

- **Style Consultation Interface** - Request outfit recommendations
- **User Selection** - Choose from available users
- **Agent Workflow Display** - See real-time agent interactions
- **Outfit Results** - View curated outfits with justifications
- **API Integration** - Full integration with backend API

## Installation

```bash
cd ClientApp
npm install
```

## Development Server

```bash
npm start
```

Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## API Configuration

The app connects to the API at `https://localhost:7208/api`.

Update the `apiUrl` in `src/app/services/api.service.ts` if using a different server.

## Build

```bash
npm run build
```

The build artifacts will be stored in the `dist/` directory.

## Components

### StyleRequestComponent
- **Selector**: `app-style-request`
- **Features**:
  - User selection dropdown
  - Style request text area
  - Curate outfit button
  - Agent workflow display
  - Final outfit presentation
  - Error handling

## Services

### ApiService
Handles all communication with the backend API:
- User endpoints: `getUser()`, `getAllUsers()`, `createUser()`, `updateUser()`
- Product endpoints: `getProduct()`, `getAllProducts()`, `searchProducts()`
- Consultation: `curateOutfit()`

## Styling

The application uses:
- CSS Grid and Flexbox for layouts
- Color-coded agent messages (each agent has a unique color)
- Responsive design for mobile and desktop
- Material Design principles

## API Endpoints Used

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user profile
- `POST /api/users` - Create/update user
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product
- `POST /api/products/search` - Search products
- `POST /api/stylesyndicate/curate-outfit?userId={id}` - Get style recommendation

## CORS Configuration

The API is configured to allow requests from:
- `http://localhost:4200`
- `https://localhost:4200`
- `http://localhost:3000`
- `https://localhost:3000`

## Dependencies

- **@angular/core** ^17.0.0
- **@angular/common** ^17.0.0
- **@angular/platform-browser** ^17.0.0
- **@angular/forms** ^17.0.0
- **@angular/router** ^17.0.0
- **rxjs** ^7.8.0

## Future Enhancements

- [ ] Add product catalog view
- [ ] Implement user profile editor
- [ ] Add outfit history/saved outfits
- [ ] Real-time agent updates using WebSockets
- [ ] Image gallery for products
- [ ] Advanced filtering and search
- [ ] User authentication
- [ ] Shopping cart integration

## Troubleshooting

### "Cannot find module" errors
```bash
npm install
```

### API connection issues
- Ensure the backend API is running on port 7208
- Check CORS configuration in the backend
- Verify API URL in `api.service.ts`

### SSL certificate warnings
- Use `--ssl=false` flag if testing locally without HTTPS
- Or accept the self-signed certificate in your browser

## License

Internal use only.
