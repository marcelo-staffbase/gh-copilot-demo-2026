import { Request, Response, NextFunction } from 'express';
import { AlbumCreateDto, AlbumUpdateDto, ArtistCreateDto, ArtistUpdateDto } from '../types';

export const validateAlbumCreate = (req: Request, res: Response, next: NextFunction): void => {
  const dto = req.body as AlbumCreateDto;

  if (!dto) {
    res.status(400).json({ error: 'Request body is required' });
    return;
  }

  if (!dto.title || dto.title.trim() === '') {
    res.status(400).json({ error: 'Title is required and cannot be empty' });
    return;
  }

  if (!dto.image_url || dto.image_url.trim() === '') {
    res.status(400).json({ error: 'Image URL is required and cannot be empty' });
    return;
  }

  if (dto.year < 1900 || dto.year > 2100) {
    res.status(400).json({ error: 'Year must be between 1900 and 2100' });
    return;
  }

  if (dto.price < 0) {
    res.status(400).json({ error: 'Price must be non-negative' });
    return;
  }

  if (!dto.artistId || dto.artistId <= 0) {
    res.status(400).json({ error: 'Valid Artist ID is required' });
    return;
  }

  next();
};

export const validateAlbumUpdate = (req: Request, res: Response, next: NextFunction): void => {
  const dto = req.body as AlbumUpdateDto;

  if (!dto) {
    res.status(400).json({ error: 'Request body is required' });
    return;
  }

  if (!dto.title || dto.title.trim() === '') {
    res.status(400).json({ error: 'Title is required and cannot be empty' });
    return;
  }

  if (!dto.image_url || dto.image_url.trim() === '') {
    res.status(400).json({ error: 'Image URL is required and cannot be empty' });
    return;
  }

  if (dto.year < 1900 || dto.year > 2100) {
    res.status(400).json({ error: 'Year must be between 1900 and 2100' });
    return;
  }

  if (dto.price < 0) {
    res.status(400).json({ error: 'Price must be non-negative' });
    return;
  }

  if (!dto.artistId || dto.artistId <= 0) {
    res.status(400).json({ error: 'Valid Artist ID is required' });
    return;
  }

  next();
};

export const validateArtistCreate = (req: Request, res: Response, next: NextFunction): void => {
  const dto = req.body as ArtistCreateDto;

  if (!dto) {
    res.status(400).json({ error: 'Request body is required' });
    return;
  }

  if (!dto.name || dto.name.trim() === '') {
    res.status(400).json({ error: 'Name is required and cannot be empty' });
    return;
  }

  if (!dto.birthPlace || dto.birthPlace.trim() === '') {
    res.status(400).json({ error: 'Birth place is required and cannot be empty' });
    return;
  }

  if (dto.birthdate) {
    const birthdate = new Date(dto.birthdate);
    if (birthdate > new Date()) {
      res.status(400).json({ error: 'Birthdate cannot be in the future' });
      return;
    }
  }

  next();
};

export const validateArtistUpdate = (req: Request, res: Response, next: NextFunction): void => {
  const dto = req.body as ArtistUpdateDto;

  if (!dto) {
    res.status(400).json({ error: 'Request body is required' });
    return;
  }

  if (!dto.name || dto.name.trim() === '') {
    res.status(400).json({ error: 'Name is required and cannot be empty' });
    return;
  }

  if (!dto.birthPlace || dto.birthPlace.trim() === '') {
    res.status(400).json({ error: 'Birth place is required and cannot be empty' });
    return;
  }

  if (dto.birthdate) {
    const birthdate = new Date(dto.birthdate);
    if (birthdate > new Date()) {
      res.status(400).json({ error: 'Birthdate cannot be in the future' });
      return;
    }
  }

  next();
};
