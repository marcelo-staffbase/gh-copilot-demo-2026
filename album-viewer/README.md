# Album Viewer

A modern Vue.js 3 application built with TypeScript that displays albums from the albums API.

## Features

- 🎵 Display album collection in a beautiful grid layout
- � Multi-language support (English, French, German)
- �🎨 Modern, responsive design with gradient background
- 🖼️ Album cover images with hover effects
- 💰 Price display for each album
- 📱 Mobile-friendly responsive design
- ⚡ Built with Vue 3, TypeScript, and Vite
- 🔧 Full TypeScript support with type safety
- 📝 Modern Composition API with `<script setup>`

## Prerequisites

- Node.js (v16 or higher)
- npm or yarn
- TypeScript knowledge (helpful but not required)
- The album-api-v2 (Node.js) or albums-api (.NET) should be running on `http://localhost:3000`

## Getting Started

1. Install dependencies:
   ```bash
   npm install
   ```

2. Make sure the backend API is running:
   - For Node.js backend (album-api-v2): `npm start` in the album-api-v2 directory
   - For .NET backend (albums-api): Start the albums-api project
   - The backend should be accessible at `http://localhost:3000/api/album`

3. Start the development server:
   ```bash
   npm run dev
   ```

4. Open your browser and navigate to `http://localhost:3001`

## API Integration

The app runs on port 3001 and uses a Vite proxy to communicate with the backend API:
- Frontend URL: `http://localhost:3001`
- Frontend calls: `/albums`
- Vite proxy forwards to: `http://localhost:3000/api/album`

### Proxy Configuration

The Vite configuration (in `vite.config.ts`) includes a proxy setup:
```typescript
server: {
  port: 3001,
  proxy: {
    '/albums': {
      target: 'http://localhost:3000/api/album',
      changeOrigin: true,
      rewrite: (path) => path.replace(/^\/albums/, '')
    }
  }
}
```

This allows the frontend to make requests to `/albums` which are automatically forwarded to the backend's `/api/album` endpoint.

### API Response Format

The API should return albums in the following format:
```json
[
  {
    "id": 1,
    "title": "Album Title",
    "artist": "Artist Name",
    "price": 10.99,
    "image_url": "https://example.com/image.jpg"
  }
]
```

## Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production (with TypeScript compilation)
- `npm run preview` - Preview production build
- `npm run type-check` - Run TypeScript type checking without building

## Project Structure

```
album-viewer/
├── src/
│   ├── components/
│   │   └── AlbumCard.vue    # Individual album card component (TypeScript)
│   ├── types/
│   │   └── album.ts         # TypeScript type definitions
│   ├── App.vue              # Main app component (TypeScript)
│   └── main.ts              # App entry point (TypeScript)
├── index.html               # HTML template
├── vite.config.ts           # Vite configuration (TypeScript)
├── tsconfig.json            # TypeScript configuration
├── tsconfig.app.json        # App-specific TypeScript config
├── env.d.ts                 # Environment type declarations
└── package.json             # Dependencies and scripts
```

## Technologies Used

- Vue 3 (Composition API with `<script setup>`)
- TypeScript (Static type checking and better developer experience)
- Vue I18n (Internationalization framework)
- Vite (Build tool with TypeScript support)
- Axios (HTTP client with TypeScript generics)
- CSS3 (Grid, Flexbox, Animations)

## Internationalization

The application supports multiple languages using Vue I18n:

### Supported Languages
- **English (en)** - Default language
- **French (fr)** - Français
- **German (de)** - Deutsch

### Language Selector
A language selector is available in the header of the application. Changing the language updates:
- All UI text and labels
- Button text
- Error messages
- Date formatting (locale-aware)

### Translation Files
Translation files are located in `src/locales/`:
- `en.ts` - English translations
- `fr.ts` - French translations
- `de.ts` - German translations

### Adding New Languages
To add a new language:
1. Create a new translation file in `src/locales/` (e.g., `es.ts` for Spanish)
2. Add the language to `src/locales/index.ts`
3. Update the language selector in `App.vue`

## TypeScript Features

This application leverages TypeScript for enhanced development experience:

- **Type Safety**: All components, functions, and data structures are strongly typed
- **Interface Definitions**: Clear contracts for data structures (Album interface)
- **Better IDE Support**: Enhanced IntelliSense, auto-completion, and error detection
- **Compile-time Error Checking**: Catch errors before runtime
- **Modern Vue 3 Syntax**: Uses `<script setup lang="ts">` for optimal TypeScript integration

## Features in Detail

### Album Cards
Each album is displayed in a card with:
- Album cover image
- Title and artist information
- Price display
- Hover effects with play button overlay
- Add to Cart and Preview buttons

### Responsive Design
The app adapts to different screen sizes:
- Desktop: Multi-column grid layout
- Mobile: Single column layout with stacked buttons

### Error Handling
- Loading spinner while fetching data
- Error message with retry button if API is unavailable
- Fallback placeholder image for broken album covers
