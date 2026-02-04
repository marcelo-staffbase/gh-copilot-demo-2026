using albums_api.Controllers;
using albums_api.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace albums_api.Tests
{
    public class ArtistControllerTests : IDisposable
    {
        private readonly ArtistController _controller;

        public ArtistControllerTests()
        {
            _controller = new ArtistController();
            ResetArtistData();
        }

        public void Dispose()
        {
            ResetArtistData();
        }

        private void ResetArtistData()
        {
            typeof(Artist).GetField("_artists", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?
                .SetValue(null, new List<Artist>
                {
                    new Artist(1, "Daprize", new DateTime(2020, 1, 15), "Cloud City, USA"),
                    new Artist(2, "The Blue-Green Stripes", new DateTime(2018, 6, 22), "Seattle, WA"),
                    new Artist(3, "KEDA Club", new DateTime(2019, 3, 10), "London, UK"),
                    new Artist(4, "MegaDNS", new DateTime(2017, 11, 5), "San Francisco, CA"),
                    new Artist(5, "V is for VNET", new DateTime(2016, 8, 30), "Austin, TX"),
                    new Artist(6, "Guns N Probeses", new DateTime(2015, 4, 12), "Los Angeles, CA"),
                    new Artist(7, "Pipeline Pilots", new DateTime(2021, 9, 18), "New York, NY")
                });

            typeof(Artist).GetField("_nextId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?
                .SetValue(null, 8);
        }

        #region GET Tests

        [Fact]
        public void Get_ReturnsAllArtists()
        {
            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var artists = Assert.IsAssignableFrom<List<Artist>>(okResult.Value);
            Assert.Equal(7, artists.Count);
        }

        #endregion

        #region GET by ID Tests

        [Fact]
        public void GetById_WithValidId_ReturnsArtist()
        {
            // Act
            var result = _controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var artist = Assert.IsType<Artist>(okResult.Value);
            Assert.Equal(1, artist.Id);
            Assert.Equal("Daprize", artist.Name);
            Assert.Equal("Cloud City, USA", artist.BirthPlace);
        }

        [Fact]
        public void GetById_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = _controller.Get(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region Search by Name Tests

        [Fact]
        public void SearchByName_WithValidName_ReturnsArtist()
        {
            // Act
            var result = _controller.SearchByName("Daprize");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var artist = Assert.IsType<Artist>(okResult.Value);
            Assert.Equal("Daprize", artist.Name);
        }

        [Fact]
        public void SearchByName_CaseInsensitive_ReturnsArtist()
        {
            // Act
            var result = _controller.SearchByName("daprize");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var artist = Assert.IsType<Artist>(okResult.Value);
            Assert.Equal("Daprize", artist.Name);
        }

        [Fact]
        public void SearchByName_WithNonExistentName_ReturnsNotFound()
        {
            // Act
            var result = _controller.SearchByName("NonExistent");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void SearchByName_WithEmptyName_ReturnsBadRequest()
        {
            // Act
            var result = _controller.SearchByName("");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name parameter is required", badRequestResult.Value);
        }

        [Fact]
        public void SearchByName_WithNullName_ReturnsBadRequest()
        {
            // Act
            var result = _controller.SearchByName(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name parameter is required", badRequestResult.Value);
        }

        #endregion

        #region POST (Create) Tests

        [Fact]
        public void Post_WithValidData_CreatesArtist()
        {
            // Arrange
            var newArtistDto = new ArtistCreateDto
            {
                Name = "New Artist",
                Birthdate = new DateTime(2000, 1, 1),
                BirthPlace = "Test City"
            };

            // Act
            var result = _controller.Post(newArtistDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var artist = Assert.IsType<Artist>(createdResult.Value);
            Assert.Equal("New Artist", artist.Name);
            Assert.Equal(new DateTime(2000, 1, 1), artist.Birthdate);
            Assert.Equal("Test City", artist.BirthPlace);
        }

        [Fact]
        public void Post_WithNullBirthdate_CreatesArtist()
        {
            // Arrange
            var newArtistDto = new ArtistCreateDto
            {
                Name = "New Artist",
                Birthdate = null,
                BirthPlace = "Test City"
            };

            // Act
            var result = _controller.Post(newArtistDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var artist = Assert.IsType<Artist>(createdResult.Value);
            Assert.Equal("New Artist", artist.Name);
            Assert.Null(artist.Birthdate);
        }

        [Fact]
        public void Post_WithNullData_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Post(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Artist data is required", badRequestResult.Value);
        }

        [Fact]
        public void Post_WithEmptyName_ReturnsBadRequest()
        {
            // Arrange
            var newArtistDto = new ArtistCreateDto
            {
                Name = "",
                BirthPlace = "Test City"
            };

            // Act
            var result = _controller.Post(newArtistDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name and BirthPlace are required", badRequestResult.Value);
        }

        [Fact]
        public void Post_WithFutureBirthdate_ReturnsBadRequest()
        {
            // Arrange
            var newArtistDto = new ArtistCreateDto
            {
                Name = "Future Artist",
                Birthdate = DateTime.Now.AddYears(1),
                BirthPlace = "Test City"
            };

            // Act
            var result = _controller.Post(newArtistDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Birthdate cannot be in the future", badRequestResult.Value);
        }

        #endregion

        #region PUT (Update) Tests

        [Fact]
        public void Put_WithValidData_UpdatesArtist()
        {
            // Arrange
            var updateDto = new ArtistUpdateDto
            {
                Name = "Updated Name",
                Birthdate = new DateTime(2020, 12, 31),
                BirthPlace = "Updated City"
            };

            // Act
            var result = _controller.Put(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var artist = Assert.IsType<Artist>(okResult.Value);
            Assert.Equal(1, artist.Id);
            Assert.Equal("Updated Name", artist.Name);
            Assert.Equal(new DateTime(2020, 12, 31), artist.Birthdate);
            Assert.Equal("Updated City", artist.BirthPlace);
        }

        [Fact]
        public void Put_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new ArtistUpdateDto
            {
                Name = "Updated Name",
                BirthPlace = "Updated City"
            };

            // Act
            var result = _controller.Put(999, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Put_WithNullData_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Put(1, null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Artist data is required", badRequestResult.Value);
        }

        [Fact]
        public void Put_WithEmptyBirthPlace_ReturnsBadRequest()
        {
            // Arrange
            var updateDto = new ArtistUpdateDto
            {
                Name = "Updated Name",
                BirthPlace = ""
            };

            // Act
            var result = _controller.Put(1, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name and BirthPlace are required", badRequestResult.Value);
        }

        [Fact]
        public void Put_WithFutureBirthdate_ReturnsBadRequest()
        {
            // Arrange
            var updateDto = new ArtistUpdateDto
            {
                Name = "Updated Name",
                Birthdate = DateTime.Now.AddYears(1),
                BirthPlace = "Updated City"
            };

            // Act
            var result = _controller.Put(1, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Birthdate cannot be in the future", badRequestResult.Value);
        }

        #endregion

        #region DELETE Tests

        [Fact]
        public void Delete_WithValidId_DeletesArtist()
        {
            // Act
            var result = _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify it's deleted
            var getResult = _controller.Get(1);
            Assert.IsType<NotFoundResult>(getResult);
        }

        [Fact]
        public void Delete_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_AlreadyDeletedArtist_ReturnsNotFound()
        {
            // First delete
            _controller.Delete(1);

            // Try to delete again
            var result = _controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void FullCRUDWorkflow_WorksCorrectly()
        {
            // Create
            var createDto = new ArtistCreateDto
            {
                Name = "Integration Test Artist",
                Birthdate = new DateTime(1990, 5, 15),
                BirthPlace = "Integration City"
            };
            var createResult = _controller.Post(createDto);
            var createdArtist = ((CreatedAtActionResult)createResult).Value as Artist;
            Assert.NotNull(createdArtist);
            int newId = createdArtist!.Id;

            // Read
            var readResult = _controller.Get(newId);
            var readArtist = ((OkObjectResult)readResult).Value as Artist;
            Assert.NotNull(readArtist);
            Assert.Equal("Integration Test Artist", readArtist!.Name);

            // Update
            var updateDto = new ArtistUpdateDto
            {
                Name = "Updated Integration Artist",
                Birthdate = new DateTime(1991, 6, 16),
                BirthPlace = "Updated City"
            };
            var updateResult = _controller.Put(newId, updateDto);
            var updatedArtist = ((OkObjectResult)updateResult).Value as Artist;
            Assert.NotNull(updatedArtist);
            Assert.Equal("Updated Integration Artist", updatedArtist!.Name);

            // Search
            var searchResult = _controller.SearchByName("Updated Integration Artist");
            var searchedArtist = ((OkObjectResult)searchResult).Value as Artist;
            Assert.NotNull(searchedArtist);
            Assert.Equal(newId, searchedArtist!.Id);

            // Delete
            var deleteResult = _controller.Delete(newId);
            Assert.IsType<NoContentResult>(deleteResult);

            // Verify deletion
            var verifyResult = _controller.Get(newId);
            Assert.IsType<NotFoundResult>(verifyResult);
        }

        #endregion
    }
}
