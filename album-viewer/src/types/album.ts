export interface Artist {
  id: number
  name: string
  birthdate: string | null
  birthPlace: string
}

export interface Album {
  id: number
  title: string
  artist: Artist
  year: number
  price: number
  image_url: string
}
