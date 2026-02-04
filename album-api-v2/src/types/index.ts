export interface Artist {
  id: number;
  name: string;
  birthdate: string | null;
  birthPlace: string;
}

export interface Album {
  id: number;
  title: string;
  artist: Artist;
  year: number;
  price: number;
  image_url: string;
}

export interface ArtistCreateDto {
  name: string;
  birthdate?: string | null;
  birthPlace: string;
}

export interface ArtistUpdateDto {
  name: string;
  birthdate?: string | null;
  birthPlace: string;
}

export interface AlbumCreateDto {
  title: string;
  artistId: number;
  year: number;
  price: number;
  image_url: string;
}

export interface AlbumUpdateDto {
  title: string;
  artistId: number;
  year: number;
  price: number;
  image_url: string;
}
