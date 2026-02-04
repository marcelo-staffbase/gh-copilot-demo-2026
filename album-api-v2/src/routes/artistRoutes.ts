import { Router, Request, Response } from 'express';
import { dataService } from '../services/dataService';
import { validateArtistCreate, validateArtistUpdate } from '../middleware/validation';

const router = Router();

// GET /api/artist - Get all artists
router.get('/', (req: Request, res: Response) => {
  const artists = dataService.getAllArtists();
  res.json(artists);
});

// GET /api/artist/:id - Get artist by ID
router.get('/:id', (req: Request, res: Response) => {
  const id = parseInt(req.params.id);
  const artist = dataService.getArtistById(id);

  if (!artist) {
    res.status(404).json({ error: 'Artist not found' });
    return;
  }

  res.json(artist);
});

// GET /api/artist/search/name - Search artists by name
router.get('/search/name', (req: Request, res: Response) => {
  const name = req.query.name as string;

  if (!name || name.trim() === '') {
    res.status(400).json({ error: name === '' ? 'Name parameter cannot be empty' : 'Name parameter is required' });
    return;
  }

  const artists = dataService.searchArtistsByName(name);
  res.json(artists);
});

// POST /api/artist - Create new artist
router.post('/', validateArtistCreate, (req: Request, res: Response) => {
  const artist = dataService.createArtist(req.body);
  res.status(201).json(artist);
});

// PUT /api/artist/:id - Update artist
router.put('/:id', validateArtistUpdate, (req: Request, res: Response) => {
  const id = parseInt(req.params.id);
  const artist = dataService.updateArtist(id, req.body);

  if (!artist) {
    res.status(404).json({ error: 'Artist not found' });
    return;
  }

  res.json(artist);
});

// DELETE /api/artist/:id - Delete artist
router.delete('/:id', (req: Request, res: Response) => {
  const id = parseInt(req.params.id);
  const success = dataService.deleteArtist(id);

  if (!success) {
    res.status(404).json({ error: 'Artist not found' });
    return;
  }

  res.status(204).send();
});

export default router;
