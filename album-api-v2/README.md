# Album API v2

A Node.js/TypeScript REST API for managing music albums and artists. This is a rewrite of the original .NET albums-api with identical functionality and routes.

## Features

- **TypeScript**: Fully typed API with strict type checking
- **In-Memory Storage**: No database required - data persists during runtime
- **RESTful API**: Complete CRUD operations for albums and artists
- **Validation**: Comprehensive input validation middleware
- **Testing**: 52 unit tests with Jest and Supertest (100% passing)
- **CORS Enabled**: Ready for cross-origin requests

## Prerequisites

- Node.js (v18 or higher recommended)
- npm (comes with Node.js)

## Installation

```bash
npm install
```

## Available Scripts

### Development
Start the server with hot-reload using ts-node:
```bash
npm run dev
```

### Build
Compile TypeScript to JavaScript:
```bash
npm run build
```

### Production
Start the compiled application:
```bash
npm start
```

### Testing
Run all unit tests:
```bash
npm test
```

Run tests in watch mode:
```bash
npm run test:watch
```

## API Endpoints

The server runs on **http://localhost:3000** by default.

### Albums

| Method | Endpoint | Description | Query Parameters |
|--------|----------|-------------|------------------|
| GET | `/api/album` | Get all albums | `sortBy`: "title", "artist", or "price" |
| GET | `/api/album/:id` | Get album by ID | - |
| GET | `/api/album/search/year` | Search albums by year | `year`: number (required) |
| POST | `/api/album` | Create new album | - |
| PUT | `/api/album/:id` | Update album | - |
| DELETE | `/api/album/:id` | Delete album | - |

### Artists

| Method | Endpoint | Description | Query Parameters |
|--------|----------|-------------|------------------|
| GET | `/api/artist` | Get all artists | - |
| GET | `/api/artist/:id` | Get artist by ID | - |
| GET | `/api/artist/search/name` | Search artists by name | `name`: string (required, case-insensitive) |
| POST | `/api/artist` | Create new artist | - |
| PUT | `/api/artist/:id` | Update artist | - |
| DELETE | `/api/artist/:id` | Delete artist | - |

## Data Models

### Album
```typescript
{
  id: number;
  title: string;
  artist: Artist;
  year: number;          // Must be between 1900-2100
  price: number;         // Must be non-negative
  image_url: string;
}
```

### Artist
```typescript
{
  id: number;
  name: string;
  birthdate: string | null;  // ISO 8601 format, cannot be future
  birthPlace: string;
}
```

## Example Requests

### Get all albums sorted by price
```bash
curl http://localhost:3000/api/album?sortBy=price
```

### Create a new album
```bash
curl -X POST http://localhost:3000/api/album \
  -H "Content-Type: application/json" \
  -d '{
    "title": "My New Album",
    "artistId": 1,
    "year": 2024,
    "price": 14.99,
    "image_url": "https://example.com/album.jpg"
  }'
```

### Search albums by year
```bash
curl http://localhost:3000/api/album/search/year?year=2023
```

### Search artists by name
```bash
curl http://localhost:3000/api/artist/search/name?name=Daprize
```

## Validation Rules

### Album Creation/Update
- `title`: Required, cannot be empty
- `image_url`: Required, cannot be empty
- `year`: Must be between 1900 and 2100
- `price`: Must be non-negative (≥ 0)
- `artistId`: Must reference an existing artist

### Artist Creation/Update
- `name`: Required, cannot be empty
- `birthPlace`: Required, cannot be empty
- `birthdate`: Optional, but cannot be in the future if provided

## Sample Data

The API comes pre-loaded with 7 artists and 7 albums:

**Artists:**
- Daprize
- The Blue-Green Stripes
- KEDA Club
- MegaDNS
- V is for VNET
- Guns N Probeses
- Pipeline Pilots

**Albums:**
- You, Me and an App Id (Daprize, 2023)
- Seven Revision Army (The Blue-Green Stripes, 2022)
- Scale It Up (KEDA Club, 2021)
- Lost in Translation (MegaDNS, 2020)
- Lock Down Your Love (V is for VNET, 2019)
- Sweet Container O' Mine (Guns N Probeses, 2018)
- The CI/CD Experience (Pipeline Pilots, 2024)

## Testing

Run the comprehensive test suite:

```bash
npm test
```

**Test Coverage:**
- ✅ 52 tests passing
- Album CRUD operations (29 tests)
- Artist CRUD operations (23 tests)
- Validation scenarios
- Error handling (404, 400 responses)
- Sorting and searching functionality
- Integration workflows

## Project Structure

```
album-api-v2/
├── src/
│   ├── middleware/
│   │   └── validation.ts          # Request validation middleware
│   ├── routes/
│   │   ├── albumRoutes.ts         # Album endpoints
│   │   ├── albumRoutes.test.ts    # Album tests
│   │   ├── artistRoutes.ts        # Artist endpoints
│   │   └── artistRoutes.test.ts   # Artist tests
│   ├── services/
│   │   └── dataService.ts         # In-memory data store
│   ├── types/
│   │   └── index.ts               # TypeScript interfaces
│   └── server.ts                  # Express app configuration
├── dist/                          # Compiled JavaScript (after build)
├── jest.config.js                 # Jest configuration
├── package.json                   # Dependencies and scripts
├── tsconfig.json                  # TypeScript configuration
└── README.md                      # This file
```

## Compatibility

This API is designed to be a drop-in replacement for the original .NET albums-api and is fully compatible with the VueJS album-viewer frontend application.

## License

MIT
