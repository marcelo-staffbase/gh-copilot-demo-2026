namespace albums_api.Models
{
    public record Artist(int Id, string Name, DateTime? Birthdate, string BirthPlace)
    {
        private static List<Artist> _artists = new List<Artist>(){
            new Artist(1, "Daprize", new DateTime(2020, 1, 15), "Cloud City, USA"),
            new Artist(2, "The Blue-Green Stripes", new DateTime(2018, 6, 22), "Seattle, WA"),
            new Artist(3, "KEDA Club", new DateTime(2019, 3, 10), "London, UK"),
            new Artist(4, "MegaDNS", new DateTime(2017, 11, 5), "San Francisco, CA"),
            new Artist(5, "V is for VNET", new DateTime(2016, 8, 30), "Austin, TX"),
            new Artist(6, "Guns N Probeses", new DateTime(2015, 4, 12), "Los Angeles, CA"),
            new Artist(7, "Pipeline Pilots", new DateTime(2021, 9, 18), "New York, NY")
        };
        
        private static int _nextId = 8;

        public static List<Artist> GetAll()
        {
            return new List<Artist>(_artists);
        }

        public static Artist? GetById(int id)
        {
            return _artists.FirstOrDefault(a => a.Id == id);
        }
        
        public static Artist? GetByName(string name)
        {
            return _artists.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        
        public static Artist Create(string name, DateTime? birthdate, string birthPlace)
        {
            var newArtist = new Artist(_nextId++, name, birthdate, birthPlace);
            _artists.Add(newArtist);
            return newArtist;
        }
        
        public static Artist? Update(int id, string name, DateTime? birthdate, string birthPlace)
        {
            var index = _artists.FindIndex(a => a.Id == id);
            if (index == -1)
            {
                return null;
            }
            
            var updatedArtist = new Artist(id, name, birthdate, birthPlace);
            _artists[index] = updatedArtist;
            return updatedArtist;
        }
        
        public static bool Delete(int id)
        {
            var artist = _artists.FirstOrDefault(a => a.Id == id);
            if (artist == null)
            {
                return false;
            }
            
            _artists.Remove(artist);
            return true;
        }
    }
}
