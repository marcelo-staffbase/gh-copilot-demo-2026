namespace albums_api.Models
{
    public record Album(int Id, string Title, string Artist, int Year, double Price, string Image_url)
    {
        private static List<Album> _albums = new List<Album>(){
            new Album(1, "You, Me and an App Id", "Daprize", 2023, 10.99, "https://aka.ms/albums-daprlogo"),
            new Album(2, "Seven Revision Army", "The Blue-Green Stripes", 2022, 13.99, "https://aka.ms/albums-containerappslogo"),
            new Album(3, "Scale It Up", "KEDA Club", 2021, 13.99, "https://aka.ms/albums-kedalogo"),
            new Album(4, "Lost in Translation", "MegaDNS", 2020, 12.99,"https://aka.ms/albums-envoylogo"),
            new Album(5, "Lock Down Your Love", "V is for VNET", 2019, 12.99, "https://aka.ms/albums-vnetlogo"),
            new Album(6, "Sweet Container O' Mine", "Guns N Probeses", 2018, 14.99, "https://aka.ms/albums-containerappslogo"),
            new Album(7, "The CI/CD Experience", "Pipeline Pilots", 2024, 15.99, "https://aka.ms/albums-azurepipelineslogo")
        };
        
        private static int _nextId = 8;

        public static List<Album> GetAll()
        {
            return new List<Album>(_albums);
        }

        public static Album? GetById(int id)
        {
            return _albums.FirstOrDefault(a => a.Id == id);
        }
        
        public static List<Album> GetByYear(int year)
        {
            return _albums.Where(a => a.Year == year).ToList();
        }
        
        public static Album Create(string title, string artist, int year, double price, string imageUrl)
        {
            var newAlbum = new Album(_nextId++, title, artist, year, price, imageUrl);
            _albums.Add(newAlbum);
            return newAlbum;
        }
        
        public static Album? Update(int id, string title, string artist, int year, double price, string imageUrl)
        {
            var index = _albums.FindIndex(a => a.Id == id);
            if (index == -1)
            {
                return null;
            }
            
            var updatedAlbum = new Album(id, title, artist, year, price, imageUrl);
            _albums[index] = updatedAlbum;
            return updatedAlbum;
        }
        
        public static bool Delete(int id)
        {
            var album = _albums.FirstOrDefault(a => a.Id == id);
            if (album == null)
            {
                return false;
            }
            
            _albums.Remove(album);
            return true;
        }
    }
}
