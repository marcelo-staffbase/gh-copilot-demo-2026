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
    /// <summary>
    /// Controller for managing albums
    /// It is responsible for handling album-related API requests such as retrieving album lists and details.
    /// </summary>
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
                "artist" => albums.OrderBy(a => a.Artist.Name).ToList(),
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

        // GET: api/album/search?year=2023
        [HttpGet("search")]
        public IActionResult SearchByYear([FromQuery] int? year)
        {
            if (!year.HasValue)
            {
                return BadRequest("Year parameter is required");
            }
            
            var albums = Album.GetByYear(year.Value);
            return Ok(albums);
        }

        // POST api/<AlbumController>
        [HttpPost]
        public IActionResult Post([FromBody] AlbumCreateDto albumDto)
        {
            if (albumDto == null)
            {
                return BadRequest("Album data is required");
            }

            if (string.IsNullOrWhiteSpace(albumDto.Title) || 
                string.IsNullOrWhiteSpace(albumDto.Image_url))
            {
                return BadRequest("Title and Image URL are required");
            }

            if (albumDto.Year < 1900 || albumDto.Year > 2100)
            {
                return BadRequest("Year must be between 1900 and 2100");
            }

            if (albumDto.Price < 0)
            {
                return BadRequest("Price must be non-negative");
            }

            if (albumDto.ArtistId <= 0)
            {
                return BadRequest("Valid Artist ID is required");
            }

            try
            {
                var newAlbum = Album.Create(
                    albumDto.Title, 
                    albumDto.ArtistId, 
                    albumDto.Year, 
                    albumDto.Price, 
                    albumDto.Image_url
                );

                return CreatedAtAction(nameof(Get), new { id = newAlbum.Id }, newAlbum);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<AlbumController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] AlbumUpdateDto albumDto)
        {
            if (albumDto == null)
            {
                return BadRequest("Album data is required");
            }

            if (string.IsNullOrWhiteSpace(albumDto.Title) || 
                string.IsNullOrWhiteSpace(albumDto.Image_url))
            {
                return BadRequest("Title and Image URL are required");
            }

            if (albumDto.Year < 1900 || albumDto.Year > 2100)
            {
                return BadRequest("Year must be between 1900 and 2100");
            }

            if (albumDto.Price < 0)
            {
                return BadRequest("Price must be non-negative");
            }

            if (albumDto.ArtistId <= 0)
            {
                return BadRequest("Valid Artist ID is required");
            }

            try
            {
                var updatedAlbum = Album.Update(
                    id,
                    albumDto.Title, 
                    albumDto.ArtistId, 
                    albumDto.Year, 
                    albumDto.Price, 
                    albumDto.Image_url
                );

                if (updatedAlbum == null)
                {
                    return NotFound();
                }

                return Ok(updatedAlbum);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<AlbumController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = Album.Delete(id);
            
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

    }

    // DTOs for request validation
    public class AlbumCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public int ArtistId { get; set; }
        public int Year { get; set; }
        public double Price { get; set; }
        public string Image_url { get; set; } = string.Empty;
    }

    public class AlbumUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public int ArtistId { get; set; }
        public int Year { get; set; }
        public double Price { get; set; }
        public string Image_url { get; set; } = string.Empty;
    }
}
