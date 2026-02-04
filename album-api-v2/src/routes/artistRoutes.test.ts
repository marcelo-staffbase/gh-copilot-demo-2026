import request from 'supertest';
import app from '../server';
import { dataService } from '../services/dataService';

describe('Artist API Tests', () => {
  beforeEach(() => {
    dataService.reset();
  });

  describe('GET /api/artist', () => {
    it('should get all artists', async () => {
      const response = await request(app).get('/api/artist');
      expect(response.status).toBe(200);
      expect(Array.isArray(response.body)).toBe(true);
      expect(response.body.length).toBe(7);
    });
  });

  describe('GET /api/artist/:id', () => {
    it('should get artist by valid ID', async () => {
      const response = await request(app).get('/api/artist/1');
      expect(response.status).toBe(200);
      expect(response.body.id).toBe(1);
      expect(response.body.name).toBe('Daprize');
    });

    it('should return 404 for invalid ID', async () => {
      const response = await request(app).get('/api/artist/999');
      expect(response.status).toBe(404);
      expect(response.body.error).toBe('Artist not found');
    });
  });

  describe('GET /api/artist/search/name', () => {
    it('should search artists by name (case-insensitive)', async () => {
      const response = await request(app).get('/api/artist/search/name?name=daprize');
      expect(response.status).toBe(200);
      expect(response.body.length).toBe(1);
      expect(response.body[0].name).toBe('Daprize');
    });

    it('should search artists by partial name', async () => {
      const response = await request(app).get('/api/artist/search/name?name=Blue');
      expect(response.status).toBe(200);
      expect(response.body.length).toBe(1);
      expect(response.body[0].name).toBe('The Blue-Green Stripes');
    });

    it('should return empty array for non-existent artist', async () => {
      const response = await request(app).get('/api/artist/search/name?name=NonExistent');
      expect(response.status).toBe(200);
      expect(response.body.length).toBe(0);
    });

    it('should return error when name parameter is missing', async () => {
      const response = await request(app).get('/api/artist/search/name');
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Name parameter is required');
    });

    it('should return error for empty name parameter', async () => {
      const response = await request(app).get('/api/artist/search/name?name=');
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Name parameter cannot be empty');
    });
  });

  describe('POST /api/artist', () => {
    it('should create a new artist', async () => {
      const newArtist = {
        name: 'Test Artist',
        birthdate: '2020-01-01T00:00:00',
        birthPlace: 'Test City'
      };

      const response = await request(app).post('/api/artist').send(newArtist);
      expect(response.status).toBe(201);
      expect(response.body.name).toBe('Test Artist');
      expect(response.body.id).toBe(8);
    });

    it('should create artist with null birthdate', async () => {
      const newArtist = {
        name: 'Test Artist',
        birthdate: null,
        birthPlace: 'Test City'
      };

      const response = await request(app).post('/api/artist').send(newArtist);
      expect(response.status).toBe(201);
      expect(response.body.birthdate).toBeNull();
    });

    it('should return error for null data', async () => {
      const response = await request(app).post('/api/artist').send(null as any);
      expect(response.status).toBe(400);
    });

    it('should return error for empty name', async () => {
      const newArtist = {
        name: '',
        birthdate: '2020-01-01T00:00:00',
        birthPlace: 'Test City'
      };

      const response = await request(app).post('/api/artist').send(newArtist);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Name');
    });

    it('should return error for empty birthPlace', async () => {
      const newArtist = {
        name: 'Test Artist',
        birthdate: '2020-01-01T00:00:00',
        birthPlace: ''
      };

      const response = await request(app).post('/api/artist').send(newArtist);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Birth place');
    });

    it('should return error for future birthdate', async () => {
      const futureDate = new Date();
      futureDate.setFullYear(futureDate.getFullYear() + 1);

      const newArtist = {
        name: 'Test Artist',
        birthdate: futureDate.toISOString(),
        birthPlace: 'Test City'
      };

      const response = await request(app).post('/api/artist').send(newArtist);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Birthdate cannot be in the future');
    });
  });

  describe('PUT /api/artist/:id', () => {
    it('should update an existing artist', async () => {
      const updateData = {
        name: 'Updated Artist',
        birthdate: '2021-05-15T00:00:00',
        birthPlace: 'Updated City'
      };

      const response = await request(app).put('/api/artist/1').send(updateData);
      expect(response.status).toBe(200);
      expect(response.body.name).toBe('Updated Artist');
      expect(response.body.birthPlace).toBe('Updated City');
    });

    it('should return 404 for non-existent artist', async () => {
      const updateData = {
        name: 'Updated Artist',
        birthdate: '2021-05-15T00:00:00',
        birthPlace: 'Updated City'
      };

      const response = await request(app).put('/api/artist/999').send(updateData);
      expect(response.status).toBe(404);
      expect(response.body.error).toBe('Artist not found');
    });

    it('should return error for null data', async () => {
      const response = await request(app).put('/api/artist/1').send(null as any);
      expect(response.status).toBe(400);
    });

    it('should return error for empty birthPlace', async () => {
      const updateData = {
        name: 'Updated Artist',
        birthdate: '2021-05-15T00:00:00',
        birthPlace: ''
      };

      const response = await request(app).put('/api/artist/1').send(updateData);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Birth place');
    });

    it('should return error for future birthdate', async () => {
      const futureDate = new Date();
      futureDate.setFullYear(futureDate.getFullYear() + 1);

      const updateData = {
        name: 'Updated Artist',
        birthdate: futureDate.toISOString(),
        birthPlace: 'Test City'
      };

      const response = await request(app).put('/api/artist/1').send(updateData);
      expect(response.status).toBe(400);
      expect(response.body.error).toContain('Birthdate cannot be in the future');
    });
  });

  describe('DELETE /api/artist/:id', () => {
    it('should delete an existing artist', async () => {
      const response = await request(app).delete('/api/artist/1');
      expect(response.status).toBe(204);

      const getResponse = await request(app).get('/api/artist/1');
      expect(getResponse.status).toBe(404);
    });

    it('should return 404 for non-existent artist', async () => {
      const response = await request(app).delete('/api/artist/999');
      expect(response.status).toBe(404);
      expect(response.body.error).toBe('Artist not found');
    });

    it('should return 404 for double deletion', async () => {
      await request(app).delete('/api/artist/1');
      const response = await request(app).delete('/api/artist/1');
      expect(response.status).toBe(404);
    });
  });

  describe('Integration Tests', () => {
    it('should perform full CRUD workflow with search', async () => {
      // Create
      const newArtist = {
        name: 'Integration Test Artist',
        birthdate: '2020-06-15T00:00:00',
        birthPlace: 'Integration City'
      };
      const createResponse = await request(app).post('/api/artist').send(newArtist);
      expect(createResponse.status).toBe(201);
      const artistId = createResponse.body.id;

      // Read
      const getResponse = await request(app).get(`/api/artist/${artistId}`);
      expect(getResponse.status).toBe(200);
      expect(getResponse.body.name).toBe('Integration Test Artist');

      // Search
      const searchResponse = await request(app).get('/api/artist/search/name?name=Integration');
      expect(searchResponse.status).toBe(200);
      expect(searchResponse.body.length).toBe(1);

      // Update
      const updateData = {
        name: 'Updated Integration Artist',
        birthdate: '2021-07-20T00:00:00',
        birthPlace: 'Updated City'
      };
      const updateResponse = await request(app).put(`/api/artist/${artistId}`).send(updateData);
      expect(updateResponse.status).toBe(200);
      expect(updateResponse.body.name).toBe('Updated Integration Artist');

      // Delete
      const deleteResponse = await request(app).delete(`/api/artist/${artistId}`);
      expect(deleteResponse.status).toBe(204);

      // Verify deletion
      const verifyResponse = await request(app).get(`/api/artist/${artistId}`);
      expect(verifyResponse.status).toBe(404);
    });
  });
});
