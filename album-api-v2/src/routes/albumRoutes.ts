import { Router, Request, Response } from 'express';
import { dataService } from '../services/dataService';
import { validateAlbumCreate, validateAlbumUpdate } from '../middleware/validation';

const router = Router();

// GET /api/album - Get all albums with optional sorting
router.get('/', (req: Request, res: Response) => {
  const sortBy = req.query.sortBy as string | undefined;
  
  if (sortBy && !['title', 'artist', 'price'].includes(sortBy)) {
    res.status(400).json({ error: 'Invalid sort parameter. Use: title, artist, or price' });
    return;
  }

  const albums = dataService.getAllAlbums(sortBy);
  res.json(albums);
});

// GET /api/album/:id - Get album by ID
router.get('/:id', (req: Request, res: Response) => {
  const id = parseInt(req.params.id);
  const album = dataService.getAlbumById(id);

  if (!album) {
    res.status(404).json({ error: 'Album not found' });
    return;
  }

  res.json(album);
});

// GET /api/album/search/year - Search albums by year
router.get('/search/year', (req: Request, res: Response) => {
  const yearParam = req.query.year as string;

  if (!yearParam) {
    res.status(400).json({ error: 'Year parameter is required' });
    return;
  }

  const year = parseInt(yearParam);
  if (isNaN(year)) {
    res.status(400).json({ error: 'Year must be a valid number' });
    return;
  }

  const albums = dataService.searchAlbumsByYear(year);
  res.json(albums);
});

// POST /api/album - Create new album
router.post('/', validateAlbumCreate, (req: Request, res: Response) => {
  const album = dataService.createAlbum(req.body);

  if (!album) {
    res.status(400).json({ error: 'Artist not found with provided ID' });
    return;
  }

  res.status(201).json(album);
});

// PUT /api/album/:id - Update album
router.put('/:id', validateAlbumUpdate, (req: Request, res: Response) => {
  const id = parseInt(req.params.id);
  const result = dataService.updateAlbum(id, req.body);

  if (result === undefined) {
    res.status(404).json({ error: 'Album not found' });
    return;
  }

  if (result === null) {
    res.status(400).json({ error: 'Artist not found with provided ID' });
    return;
  }

  res.json(result);
});

// DELETE /api/album/:id - Delete album
router.delete('/:id', (req: Request, res: Response) => {
  const id = parseInt(req.params.id);
  const success = dataService.deleteAlbum(id);

  if (!success) {
    res.status(404).json({ error: 'Album not found' });
    return;
  }

  res.status(204).send();
});

export default router;
