import express from 'express';
import cors from 'cors';
import albumRoutes from './routes/albumRoutes';
import artistRoutes from './routes/artistRoutes';

const app = express();
const PORT = 3000;

// Middleware
app.use(cors());
app.use(express.json());

// Root route
app.get('/', (req, res) => {
  res.send('Hit the /api/album endpoint to retrieve a list of albums!');
});

// API Routes
app.use('/api/album', albumRoutes);
app.use('/api/artist', artistRoutes);

// Start server
if (require.main === module) {
  app.listen(PORT, () => {
    console.log(`Album API v2 is running on http://localhost:${PORT}`);
  });
}

export default app;
