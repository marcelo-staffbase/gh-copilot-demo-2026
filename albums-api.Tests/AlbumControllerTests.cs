using albums_api.Controllers;
using albums_api.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace albums_api.Tests
{
    public class AlbumControllerTests : IDisposable
    {
        private readonly AlbumController _controller;

        public AlbumControllerTests()
        {
            _controller = new AlbumController();
            // Reset the album data before each test
            ResetAlbumData();
        }

        public void Dispose()
        {
            // Clean up after each test
            ResetAlbumData();
        }

        private void ResetAlbumData()
        {
            // Clear all albums and reset to initial state
            // We'll need to add a Reset method to the Album model
            typeof(Album).GetField("_albums", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?
                .SetValue(null, new List<Album>
                {
                    new Album(1, "You, Me and an App Id", "Daprize", 2023, 10.99, "https://aka.ms/albums-daprlogo"),
                    new Album(2, "Seven Revision Army", "The Blue-Green Stripes", 2022, 13.99, "https://aka.ms/albums-containerappslogo"),
                    new Album(3, "Scale It Up", "KEDA Club", 2021, 13.99, "https://aka.ms/albums-kedalogo"),
                    new Album(4, "Lost in Translation", "MegaDNS", 2020, 12.99, "https://aka.ms/albums-envoylogo"),
                    new Album(5, "Lock Down Your Love", "V is for VNET", 2019, 12.99, "https://aka.ms/albums-vnetlogo"),
                    new Album(6, "Sweet Container O' Mine", "Guns N Probeses", 2018, 14.99, "https://aka.ms/albums-containerappslogo"),
                    new Album(7, "The CI/CD Experience", "Pipeline Pilots", 2024, 15.99, "https://aka.ms/albums-azurepipelineslogo")
                });

            typeof(Album).GetField("_nextId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?
                .SetValue(null, 8);
        }

        #region GET Tests

        [Fact]
        public void Get_ReturnsAllAlbums()
        {
            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            Assert.Equal(7, albums.Count);
        }

        [Fact]
        public void Get_WithSortByTitle_ReturnsSortedAlbums()
        {
            // Act
            var result = _controller.Get("title");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            Assert.Equal("Lock Down Your Love", albums[0].Title);
            Assert.Equal("Lost in Translation", albums[1].Title);
        }

        [Fact]
        public void Get_WithSortByArtist_ReturnsSortedAlbums()
        {
            // Act
            var result = _controller.Get("artist");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            Assert.Equal("Daprize", albums[0].Artist);
        }

        [Fact]
        public void Get_WithSortByPrice_ReturnsSortedAlbums()
        {
            // Act
            var result = _controller.Get("price");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            Assert.Equal(10.99, albums[0].Price);
            Assert.Equal(15.99, albums[^1].Price);
        }

        [Fact]
        public void Get_WithInvalidSortBy_ReturnsUnsortedAlbums()
        {
            // Act
            var result = _controller.Get("invalid");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            Assert.Equal(7, albums.Count);
        }

        #endregion

        #region GET by ID Tests

        [Fact]
        public void GetById_WithValidId_ReturnsAlbum()
        {
            // Act
            var result = _controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var album = Assert.IsType<Album>(okResult.Value);
            Assert.Equal(1, album.Id);
            Assert.Equal("You, Me and an App Id", album.Title);
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

        #region Search by Year Tests

        [Fact]
        public void SearchByYear_WithValidYear_ReturnsMatchingAlbums()
        {
            // Act
            var result = _controller.SearchByYear(2023);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            Assert.Single(albums);
            Assert.Equal("You, Me and an App Id", albums[0].Title);
        }

        [Fact]
        public void SearchByYear_WithYearHavingMultipleAlbums_ReturnsAll()
        {
            // First, add another album from 2023
            _controller.Post(new AlbumCreateDto
            {
                Title = "Test Album",
                Artist = "Test Artist",
                Year = 2023,
                Price = 9.99,
                Image_url = "https://example.com/image.jpg"
            });

            // Act
            var result = _controller.SearchByYear(2023);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            Assert.Equal(2, albums.Count);
        }

        [Fact]
        public void SearchByYear_WithNoMatches_ReturnsEmptyList()
        {
            // Act
            var result = _controller.SearchByYear(1999);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            Assert.Empty(albums);
        }

        [Fact]
        public void SearchByYear_WithNullYear_ReturnsBadRequest()
        {
            // Act
            var result = _controller.SearchByYear(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Year parameter is required", badRequestResult.Value);
        }

        #endregion

        #region POST (Create) Tests

        [Fact]
        public void Post_WithValidData_CreatesAlbum()
        {
            // Arrange
            var newAlbumDto = new AlbumCreateDto
            {
                Title = "New Test Album",
                Artist = "Test Artist",
                Year = 2025,
                Price = 12.99,
                Image_url = "https://example.com/test.jpg"
            };

            // Act
            var result = _controller.Post(newAlbumDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var album = Assert.IsType<Album>(createdResult.Value);
            Assert.Equal("New Test Album", album.Title);
            Assert.Equal("Test Artist", album.Artist);
            Assert.Equal(2025, album.Year);
            Assert.Equal(12.99, album.Price);
        }

        [Fact]
        public void Post_WithNullData_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Post(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Album data is required", badRequestResult.Value);
        }

        [Fact]
        public void Post_WithEmptyTitle_ReturnsBadRequest()
        {
            // Arrange
            var newAlbumDto = new AlbumCreateDto
            {
                Title = "",
                Artist = "Test Artist",
                Year = 2025,
                Price = 12.99,
                Image_url = "https://example.com/test.jpg"
            };

            // Act
            var result = _controller.Post(newAlbumDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Title, Artist, and Image URL are required", badRequestResult.Value);
        }

        [Fact]
        public void Post_WithInvalidYear_ReturnsBadRequest()
        {
            // Arrange
            var newAlbumDto = new AlbumCreateDto
            {
                Title = "Test Album",
                Artist = "Test Artist",
                Year = 1800,
                Price = 12.99,
                Image_url = "https://example.com/test.jpg"
            };

            // Act
            var result = _controller.Post(newAlbumDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Year must be between 1900 and 2100", badRequestResult.Value);
        }

        [Fact]
        public void Post_WithNegativePrice_ReturnsBadRequest()
        {
            // Arrange
            var newAlbumDto = new AlbumCreateDto
            {
                Title = "Test Album",
                Artist = "Test Artist",
                Year = 2025,
                Price = -5.99,
                Image_url = "https://example.com/test.jpg"
            };

            // Act
            var result = _controller.Post(newAlbumDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Price must be non-negative", badRequestResult.Value);
        }

        #endregion

        #region PUT (Update) Tests

        [Fact]
        public void Put_WithValidData_UpdatesAlbum()
        {
            // Arrange
            var updateDto = new AlbumUpdateDto
            {
                Title = "Updated Title",
                Artist = "Updated Artist",
                Year = 2023,
                Price = 19.99,
                Image_url = "https://example.com/updated.jpg"
            };

            // Act
            var result = _controller.Put(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var album = Assert.IsType<Album>(okResult.Value);
            Assert.Equal(1, album.Id);
            Assert.Equal("Updated Title", album.Title);
            Assert.Equal("Updated Artist", album.Artist);
            Assert.Equal(19.99, album.Price);
        }

        [Fact]
        public void Put_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new AlbumUpdateDto
            {
                Title = "Updated Title",
                Artist = "Updated Artist",
                Year = 2023,
                Price = 19.99,
                Image_url = "https://example.com/updated.jpg"
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
            Assert.Equal("Album data is required", badRequestResult.Value);
        }

        [Fact]
        public void Put_WithEmptyArtist_ReturnsBadRequest()
        {
            // Arrange
            var updateDto = new AlbumUpdateDto
            {
                Title = "Updated Title",
                Artist = "",
                Year = 2023,
                Price = 19.99,
                Image_url = "https://example.com/updated.jpg"
            };

            // Act
            var result = _controller.Put(1, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Title, Artist, and Image URL are required", badRequestResult.Value);
        }

        [Fact]
        public void Put_WithInvalidYear_ReturnsBadRequest()
        {
            // Arrange
            var updateDto = new AlbumUpdateDto
            {
                Title = "Updated Title",
                Artist = "Updated Artist",
                Year = 2200,
                Price = 19.99,
                Image_url = "https://example.com/updated.jpg"
            };

            // Act
            var result = _controller.Put(1, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Year must be between 1900 and 2100", badRequestResult.Value);
        }

        #endregion

        #region DELETE Tests

        [Fact]
        public void Delete_WithValidId_DeletesAlbum()
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
        public void Delete_AlreadyDeletedAlbum_ReturnsNotFound()
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
            var createDto = new AlbumCreateDto
            {
                Title = "Integration Test Album",
                Artist = "Integration Artist",
                Year = 2026,
                Price = 14.99,
                Image_url = "https://example.com/integration.jpg"
            };
            var createResult = _controller.Post(createDto);
            var createdAlbum = ((CreatedAtActionResult)createResult).Value as Album;
            Assert.NotNull(createdAlbum);
            int newId = createdAlbum!.Id;

            // Read
            var readResult = _controller.Get(newId);
            var readAlbum = ((OkObjectResult)readResult).Value as Album;
            Assert.NotNull(readAlbum);
            Assert.Equal("Integration Test Album", readAlbum!.Title);

            // Update
            var updateDto = new AlbumUpdateDto
            {
                Title = "Updated Integration Album",
                Artist = "Updated Artist",
                Year = 2026,
                Price = 19.99,
                Image_url = "https://example.com/updated.jpg"
            };
            var updateResult = _controller.Put(newId, updateDto);
            var updatedAlbum = ((OkObjectResult)updateResult).Value as Album;
            Assert.NotNull(updatedAlbum);
            Assert.Equal("Updated Integration Album", updatedAlbum!.Title);
            Assert.Equal(19.99, updatedAlbum.Price);

            // Delete
            var deleteResult = _controller.Delete(newId);
            Assert.IsType<NoContentResult>(deleteResult);

            // Verify deletion
            var verifyResult = _controller.Get(newId);
            Assert.IsType<NotFoundResult>(verifyResult);
        }

        [Fact]
        public void SearchAfterCreate_FindsNewAlbum()
        {
            // Create an album with a unique year
            var createDto = new AlbumCreateDto
            {
                Title = "2026 Album",
                Artist = "Future Artist",
                Year = 2026,
                Price = 20.99,
                Image_url = "https://example.com/2026.jpg"
            };
            _controller.Post(createDto);

            // Search for it
            var searchResult = _controller.SearchByYear(2026);
            var albums = ((OkObjectResult)searchResult).Value as List<Album>;
            
            Assert.NotNull(albums);
            Assert.Single(albums!);
            Assert.Equal("2026 Album", albums[0].Title);
        }

        #endregion
    }
}
