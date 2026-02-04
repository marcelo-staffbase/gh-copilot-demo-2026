import request from 'supertest';
import app from '../server';
import { dataService } from '../services/dataService';

describe('Album API Tests', () => {
  beforeEach(() => {
    dataService.reset();
  });

  describe('GET /api/album', () => {
    it('should get all albums', async () => {
      const response = await request(app).get('/api/album');
      expect(response.status).toBe(200);
      expect(Array.isArray(response.body)).toBe(true);
      expect(response.body.length).toBe(7);
    });

    it('should sort albums by title', async () => {
      const response = await request(app).get('/api/album?sortBy=title');
      expect(response.status).toBe(200);
      expect(response.body[0].title).toBe('Lock Down Your Love');
    });

    it('should sort albums by artist', async () => {
      const response = await request(app).get('/api/album?sortBy=artist');
      expect(response.status).toBe(200);
      expect(response.body[0].artist.name).toBe('Daprize');
    });

    it('should sort albums by price', async () => {
      const response = await request(app).get('/api/album?sortBy=price');
      expect(response.status).toBe(200);
      expect(response.body[0].price).toBe(10.99);
    });

    it('should return error for invalid sort parameter', async () => {
      const response = await request(app).get('/api/album?sortBy=invalid');
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Invalid sort parameter');
    });
  });

  describe('GET /api/album/:id', () => {
    it('should get album by valid ID', async () => {
      const response = await request(app).get('/api/album/1');
      expect(response.status).toBe(200);
      expect(response.body.id).toBe(1);
      expect(response.body.title).toBe('You, Me and an App Id');
    });

    it('should return 404 for invalid ID', async () => {
      const response = await request(app).get('/api/album/999');
      expect(response.status).toBe(404);
      expect(response.body.error).toBe('Album not found');
    });
  });

  describe('GET /api/album/search/year', () => {
    it('should search albums by year', async () => {
      const response = await request(app).get('/api/album/search/year?year=2023');
      expect(response.status).toBe(200);
      expect(response.body.length).toBe(1);
      expect(response.body[0].year).toBe(2023);
    });

    it('should return multiple albums for same year', async () => {
      const response = await request(app).get('/api/album/search/year?year=2021');
      expect(response.status).toBe(200);
      expect(response.body.length).toBeGreaterThanOrEqual(1);
    });

    it('should return empty array for non-existent year', async () => {
      const response = await request(app).get('/api/album/search/year?year=1999');
      expect(response.status).toBe(200);
      expect(response.body.length).toBe(0);
    });

    it('should return error when year parameter is missing', async () => {
      const response = await request(app).get('/api/album/search/year');
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Year parameter is required');
    });
  });

  describe('POST /api/album', () => {
    it('should create a new album', async () => {
      const newAlbum = {
        title: 'Test Album',
        artistId: 1,
        year: 2023,
        price: 9.99,
        image_url: 'https://example.com/image.jpg'
      };

      const response = await request(app).post('/api/album').send(newAlbum);
      expect(response.status).toBe(201);
      expect(response.body.title).toBe('Test Album');
      expect(response.body.id).toBe(8);
    });

    it('should return error for null data', async () => {
      const response = await request(app).post('/api/album').send(null as any);
      expect(response.status).toBe(400);
    });

    it('should return error for empty title', async () => {
      const newAlbum = {
        title: '',
        artistId: 1,
        year: 2023,
        price: 9.99,
        image_url: 'https://example.com/image.jpg'
      };

      const response = await request(app).post('/api/album').send(newAlbum);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Title');
    });

    it('should return error for empty image URL', async () => {
      const newAlbum = {
        title: 'Test Album',
        artistId: 1,
        year: 2023,
        price: 9.99,
        image_url: ''
      };

      const response = await request(app).post('/api/album').send(newAlbum);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Image URL');
    });

    it('should return error for year below 1900', async () => {
      const newAlbum = {
        title: 'Test Album',
        artistId: 1,
        year: 1899,
        price: 9.99,
        image_url: 'https://example.com/image.jpg'
      };

      const response = await request(app).post('/api/album').send(newAlbum);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Year');
    });

    it('should return error for year above 2100', async () => {
      const newAlbum = {
        title: 'Test Album',
        artistId: 1,
        year: 2101,
        price: 9.99,
        image_url: 'https://example.com/image.jpg'
      };

      const response = await request(app).post('/api/album').send(newAlbum);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Year');
    });

    it('should return error for negative price', async () => {
      const newAlbum = {
        title: 'Test Album',
        artistId: 1,
        year: 2023,
        price: -5,
        image_url: 'https://example.com/image.jpg'
      };

      const response = await request(app).post('/api/album').send(newAlbum);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Price');
    });

    it('should return error for invalid artist ID', async () => {
      const newAlbum = {
        title: 'Test Album',
        artistId: 999,
        year: 2023,
        price: 9.99,
        image_url: 'https://example.com/image.jpg'
      };

      const response = await request(app).post('/api/album').send(newAlbum);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Artist not found');
    });
  });

  describe('PUT /api/album/:id', () => {
    it('should update an existing album', async () => {
      const updateData = {
        title: 'Updated Album',
        artistId: 2,
        year: 2024,
        price: 19.99,
        image_url: 'https://example.com/updated.jpg'
      };

      const response = await request(app).put('/api/album/1').send(updateData);
      expect(response.status).toBe(200);
      expect(response.body.title).toBe('Updated Album');
      expect(response.body.artist.id).toBe(2);
    });

    it('should return 404 for non-existent album', async () => {
      const updateData = {
        title: 'Updated Album',
        artistId: 1,
        year: 2024,
        price: 19.99,
        image_url: 'https://example.com/updated.jpg'
      };

      const response = await request(app).put('/api/album/999').send(updateData);
      expect(response.status).toBe(404);
      expect(response.body.error).toBe('Album not found');
    });

    it('should return error for null data', async () => {
      const response = await request(app).put('/api/album/1').send(null as any);
      expect(response.status).toBe(400);
    });

    it('should return error for invalid artist ID', async () => {
      const updateData = {
        title: 'Updated Album',
        artistId: 999,
        year: 2024,
        price: 19.99,
        image_url: 'https://example.com/updated.jpg'
      };

      const response = await request(app).put('/api/album/1').send(updateData);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Artist not found');
    });

    it('should return error for invalid year', async () => {
      const updateData = {
        title: 'Updated Album',
        artistId: 1,
        year: 2101,
        price: 19.99,
        image_url: 'https://example.com/updated.jpg'
      };

      const response = await request(app).put('/api/album/1').send(updateData);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Year');
    });
  });

  describe('DELETE /api/album/:id', () => {
    it('should delete an existing album', async () => {
      const response = await request(app).delete('/api/album/1');
      expect(response.status).toBe(204);

      const getResponse = await request(app).get('/api/album/1');
      expect(getResponse.status).toBe(404);
    });

    it('should return 404 for non-existent album', async () => {
      const response = await request(app).delete('/api/album/999');
      expect(response.status).toBe(404);
      expect(response.body.error).toBe('Album not found');
    });

    it('should return 404 for double deletion', async () => {
      await request(app).delete('/api/album/1');
      const response = await request(app).delete('/api/album/1');
      expect(response.status).toBe(404);
    });
  });

  describe('Integration Tests', () => {
    it('should perform full CRUD workflow', async () => {
      // Create
      const newAlbum = {
        title: 'Integration Test Album',
        artistId: 1,
        year: 2023,
        price: 14.99,
        image_url: 'https://example.com/test.jpg'
      };
      const createResponse = await request(app).post('/api/album').send(newAlbum);
      expect(createResponse.status).toBe(201);
      const albumId = createResponse.body.id;

      // Read
      const getResponse = await request(app).get(`/api/album/${albumId}`);
      expect(getResponse.status).toBe(200);
      expect(getResponse.body.title).toBe('Integration Test Album');

      // Update
      const updateData = {
        title: 'Updated Integration Test',
        artistId: 2,
        year: 2024,
        price: 16.99,
        image_url: 'https://example.com/updated.jpg'
      };
      const updateResponse = await request(app).put(`/api/album/${albumId}`).send(updateData);
      expect(updateResponse.status).toBe(200);
      expect(updateResponse.body.title).toBe('Updated Integration Test');

      // Delete
      const deleteResponse = await request(app).delete(`/api/album/${albumId}`);
      expect(deleteResponse.status).toBe(204);

      // Verify deletion
      const verifyResponse = await request(app).get(`/api/album/${albumId}`);
      expect(verifyResponse.status).toBe(404);
    });

    it('should search after creating new album', async () => {
      const newAlbum = {
        title: 'Search Test Album',
        artistId: 1,
        year: 2025,
        price: 12.99,
        image_url: 'https://example.com/search.jpg'
      };
      await request(app).post('/api/album').send(newAlbum);

      const searchResponse = await request(app).get('/api/album/search/year?year=2025');
      expect(searchResponse.status).toBe(200);
      expect(searchResponse.body.length).toBe(1);
      expect(searchResponse.body[0].title).toBe('Search Test Album');
    });
  });
});
