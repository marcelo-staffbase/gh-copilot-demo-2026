using albums_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace albums_api.Controllers
{
    [Route("albums")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        // GET: api/album
        [HttpGet]
        public IActionResult Get([FromQuery] string? sortBy = null)
        {
            var albums = Album.GetAll();
            
            albums = SortAlbums(albums, sortBy);

            return Ok(albums);
        }
        
        /// <summary>
        /// Sorts albums by the specified field (title, artist, or price)
        /// </summary>
        /// <param name="albums">List of albums to sort</param>
        /// <param name="sortBy">Field to sort by: "title", "artist", or "price"</param>
        /// <returns>Sorted list of albums</returns>
        private List<Album> SortAlbums(List<Album> albums, string? sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return albums;
            }
            
            return sortBy.ToLower() switch
            {
                "title" => albums.OrderBy(a => a.Title).ToList(),
                "artist" => albums.OrderBy(a => a.Artist).ToList(),
                "price" => albums.OrderBy(a => a.Price).ToList(),
                _ => albums
            };
        }

        // GET api/<AlbumController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var album = Album.GetById(id);
            
            if (album == null)
            {
                return NotFound();
            }
            
            return Ok(album);
        }

    }
}
