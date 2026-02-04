using albums_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace albums_api.Controllers
{
    [Route("artists")]
    [ApiController]
    /// <summary>
    /// Controller for managing artists
    /// </summary>
    public class ArtistController : ControllerBase
    {
        // GET: api/artists
        [HttpGet]
        public IActionResult Get()
        {
            var artists = Artist.GetAll();
            return Ok(artists);
        }

        // GET api/artists/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var artist = Artist.GetById(id);
            
            if (artist == null)
            {
                return NotFound();
            }
            
            return Ok(artist);
        }

        // GET api/artists/search?name=Daprize
        [HttpGet("search")]
        public IActionResult SearchByName([FromQuery] string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name parameter is required");
            }
            
            var artist = Artist.GetByName(name);
            
            if (artist == null)
            {
                return NotFound();
            }
            
            return Ok(artist);
        }

        // POST api/artists
        [HttpPost]
        public IActionResult Post([FromBody] ArtistCreateDto artistDto)
        {
            if (artistDto == null)
            {
                return BadRequest("Artist data is required");
            }

            if (string.IsNullOrWhiteSpace(artistDto.Name) || 
                string.IsNullOrWhiteSpace(artistDto.BirthPlace))
            {
                return BadRequest("Name and BirthPlace are required");
            }

            if (artistDto.Birthdate.HasValue && artistDto.Birthdate.Value > DateTime.Now)
            {
                return BadRequest("Birthdate cannot be in the future");
            }

            var newArtist = Artist.Create(
                artistDto.Name,
                artistDto.Birthdate,
                artistDto.BirthPlace
            );

            return CreatedAtAction(nameof(Get), new { id = newArtist.Id }, newArtist);
        }

        // PUT api/artists/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ArtistUpdateDto artistDto)
        {
            if (artistDto == null)
            {
                return BadRequest("Artist data is required");
            }

            if (string.IsNullOrWhiteSpace(artistDto.Name) || 
                string.IsNullOrWhiteSpace(artistDto.BirthPlace))
            {
                return BadRequest("Name and BirthPlace are required");
            }

            if (artistDto.Birthdate.HasValue && artistDto.Birthdate.Value > DateTime.Now)
            {
                return BadRequest("Birthdate cannot be in the future");
            }

            var updatedArtist = Artist.Update(
                id,
                artistDto.Name,
                artistDto.Birthdate,
                artistDto.BirthPlace
            );

            if (updatedArtist == null)
            {
                return NotFound();
            }

            return Ok(updatedArtist);
        }

        // DELETE api/artists/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = Artist.Delete(id);
            
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }

    // DTOs for request validation
    public class ArtistCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? Birthdate { get; set; }
        public string BirthPlace { get; set; } = string.Empty;
    }

    public class ArtistUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? Birthdate { get; set; }
        public string BirthPlace { get; set; } = string.Empty;
    }
}
