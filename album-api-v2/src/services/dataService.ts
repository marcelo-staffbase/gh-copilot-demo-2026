import { Artist, Album, ArtistCreateDto, ArtistUpdateDto, AlbumCreateDto, AlbumUpdateDto } from '../types';

class DataService {
  private artists: Artist[] = [];
  private albums: Album[] = [];
  private nextArtistId = 1;
  private nextAlbumId = 1;

  constructor() {
    this.seedData();
  }

  private seedData(): void {
    // Seed Artists
    this.artists = [
      {
        id: 1,
        name: 'Daprize',
        birthdate: '2020-01-15T00:00:00',
        birthPlace: 'Cloud City, USA'
      },
      {
        id: 2,
        name: 'The Blue-Green Stripes',
        birthdate: '2018-06-22T00:00:00',
        birthPlace: 'Seattle, WA'
      },
      {
        id: 3,
        name: 'KEDA Club',
        birthdate: '2019-03-10T00:00:00',
        birthPlace: 'London, UK'
      },
      {
        id: 4,
        name: 'MegaDNS',
        birthdate: '2017-11-05T00:00:00',
        birthPlace: 'San Francisco, CA'
      },
      {
        id: 5,
        name: 'V is for VNET',
        birthdate: '2016-08-30T00:00:00',
        birthPlace: 'Austin, TX'
      },
      {
        id: 6,
        name: 'Guns N Probeses',
        birthdate: '2015-04-12T00:00:00',
        birthPlace: 'Los Angeles, CA'
      },
      {
        id: 7,
        name: 'Pipeline Pilots',
        birthdate: '2021-09-18T00:00:00',
        birthPlace: 'New York, NY'
      }
    ];

    this.nextArtistId = 8;

    // Seed Albums
    this.albums = [
      {
        id: 1,
        title: 'You, Me and an App Id',
        artist: this.artists[0],
        year: 2023,
        price: 10.99,
        image_url: 'https://aka.ms/albums-daprlogo'
      },
      {
        id: 2,
        title: 'Seven Revision Army',
        artist: this.artists[1],
        year: 2022,
        price: 13.99,
        image_url: 'https://aka.ms/albums-containerappslogo'
      },
      {
        id: 3,
        title: 'Scale It Up',
        artist: this.artists[2],
        year: 2021,
        price: 13.99,
        image_url: 'https://aka.ms/albums-kedalogo'
      },
      {
        id: 4,
        title: 'Lost in Translation',
        artist: this.artists[3],
        year: 2020,
        price: 12.99,
        image_url: 'https://aka.ms/albums-envoylogo'
      },
      {
        id: 5,
        title: 'Lock Down Your Love',
        artist: this.artists[4],
        year: 2019,
        price: 12.99,
        image_url: 'https://aka.ms/albums-vnetlogo'
      },
      {
        id: 6,
        title: 'Sweet Container O\' Mine',
        artist: this.artists[5],
        year: 2018,
        price: 14.99,
        image_url: 'https://aka.ms/albums-daprlogo'
      },
      {
        id: 7,
        title: 'The CI/CD Experience',
        artist: this.artists[6],
        year: 2024,
        price: 15.99,
        image_url: 'https://aka.ms/albums-azurepipelineslogo'
      }
    ];

    this.nextAlbumId = 8;
  }

  // Artist CRUD operations
  getAllArtists(): Artist[] {
    return [...this.artists];
  }

  getArtistById(id: number): Artist | undefined {
    return this.artists.find(a => a.id === id);
  }

  searchArtistsByName(name: string): Artist[] {
    const lowerName = name.toLowerCase();
    return this.artists.filter(a => a.name.toLowerCase().includes(lowerName));
  }

  createArtist(dto: ArtistCreateDto): Artist {
    const newArtist: Artist = {
      id: this.nextArtistId++,
      name: dto.name,
      birthdate: dto.birthdate || null,
      birthPlace: dto.birthPlace
    };
    this.artists.push(newArtist);
    return newArtist;
  }

  updateArtist(id: number, dto: ArtistUpdateDto): Artist | undefined {
    const index = this.artists.findIndex(a => a.id === id);
    if (index === -1) return undefined;

    this.artists[index] = {
      id,
      name: dto.name,
      birthdate: dto.birthdate || null,
      birthPlace: dto.birthPlace
    };
    return this.artists[index];
  }

  deleteArtist(id: number): boolean {
    const index = this.artists.findIndex(a => a.id === id);
    if (index === -1) return false;
    this.artists.splice(index, 1);
    return true;
  }

  // Album CRUD operations
  getAllAlbums(sortBy?: string): Album[] {
    const albums = [...this.albums];
    
    if (sortBy === 'title') {
      return albums.sort((a, b) => a.title.localeCompare(b.title));
    } else if (sortBy === 'artist') {
      return albums.sort((a, b) => a.artist.name.localeCompare(b.artist.name));
    } else if (sortBy === 'price') {
      return albums.sort((a, b) => a.price - b.price);
    }
    
    return albums;
  }

  getAlbumById(id: number): Album | undefined {
    return this.albums.find(a => a.id === id);
  }

  searchAlbumsByYear(year: number): Album[] {
    return this.albums.filter(a => a.year === year);
  }

  createAlbum(dto: AlbumCreateDto): Album | null {
    const artist = this.getArtistById(dto.artistId);
    if (!artist) return null;

    const newAlbum: Album = {
      id: this.nextAlbumId++,
      title: dto.title,
      artist,
      year: dto.year,
      price: dto.price,
      image_url: dto.image_url
    };
    this.albums.push(newAlbum);
    return newAlbum;
  }

  updateAlbum(id: number, dto: AlbumUpdateDto): Album | null | undefined {
    const index = this.albums.findIndex(a => a.id === id);
    if (index === -1) return undefined;

    const artist = this.getArtistById(dto.artistId);
    if (!artist) return null;

    this.albums[index] = {
      id,
      title: dto.title,
      artist,
      year: dto.year,
      price: dto.price,
      image_url: dto.image_url
    };
    return this.albums[index];
  }

  deleteAlbum(id: number): boolean {
    const index = this.albums.findIndex(a => a.id === id);
    if (index === -1) return false;
    this.albums.splice(index, 1);
    return true;
  }

  // Reset for testing
  reset(): void {
    this.artists = [];
    this.albums = [];
    this.nextArtistId = 1;
    this.nextAlbumId = 1;
    this.seedData();
  }
}

export const dataService = new DataService();
