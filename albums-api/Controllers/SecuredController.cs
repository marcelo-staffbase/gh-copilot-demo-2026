using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UnsecureApp.Services;

namespace SecureApp.Controllers
{
    /// <summary>
    /// Secure implementation of the controller with proper security measures
    /// </summary>
    public class ProductController
    {
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductController> _logger;
        private const int MaxFilePathLength = 260;
        private const int MaxProductNameLength = 100;

        public ProductController(
            IFileService fileService, 
            IConfiguration configuration,
            ILogger<ProductController> logger)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Securely reads a file with path validation to prevent path traversal attacks
        /// </summary>
        /// <param name="userInput">The file path to read</param>
        /// <returns>The file contents</returns>
        /// <exception cref="ArgumentException">Thrown when path is invalid</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when path is outside allowed directory</exception>
        public string? ReadFileSecure(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                throw new ArgumentException("File path cannot be empty", nameof(userInput));
            }

            if (userInput.Length > MaxFilePathLength)
            {
                throw new ArgumentException("File path too long", nameof(userInput));
            }

            // Prevent path traversal by validating against base directory
            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(userInput);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
            {
                _logger.LogWarning(ex, "Invalid file path provided.");
                throw new ArgumentException("The specified file path is invalid.", nameof(userInput));
            }
            string baseDirectory = Path.GetFullPath(_configuration["AllowedFileDirectory"] ?? "./data");

            // Use relative path to detect traversal attempts reliably across platforms
            string relativePath = Path.GetRelativePath(baseDirectory, fullPath);

            if (relativePath.Equals("..", StringComparison.Ordinal) ||
                relativePath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal) ||
                Path.IsPathRooted(relativePath))
            {
                _logger.LogWarning("Attempted path traversal detected. User input: {UserInput}, Resolved path: {FullPath}",
                    userInput, fullPath);
                throw new UnauthorizedAccessException("Access to the specified path is denied");
            }

            try
            {
                return _fileService.ReadFile(fullPath);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "File not found: {Path}", fullPath);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading file: {Path}", fullPath);
                throw new InvalidOperationException("An error occurred while reading the file", ex);
            }
        }

        /// <summary>
        /// Securely retrieves a product ID using parameterized queries to prevent SQL injection
        /// </summary>
        /// <param name="productName">The product name to search for</param>
        /// <returns>The product ID</returns>
        /// <exception cref="ArgumentException">Thrown when product name is invalid</exception>
        /// <exception cref="KeyNotFoundException">Thrown when product is not found</exception>
        public async Task<int> GetProductSecure(string productName)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ArgumentException("Product name cannot be empty", nameof(productName));
            }

            if (productName.Length > MaxProductNameLength)
            {
                throw new ArgumentException($"Product name cannot exceed {MaxProductNameLength} characters", 
                    nameof(productName));
            }

            // Validate connection string
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Database connection string 'DefaultConnection' is not configured");
                throw new InvalidOperationException("Database connection string is not configured");
            }

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Use parameterized query to prevent SQL injection
                const string query = "SELECT ProductId FROM Products WHERE ProductName = @ProductName";
                
                await using var command = new SqlCommand(query, connection)
                {
                    CommandType = CommandType.Text
                };

                // Add parameter with specific type and length
                command.Parameters.Add("@ProductName", SqlDbType.NVarChar, MaxProductNameLength).Value = productName;

                await using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    int productId = reader.GetInt32(0);
                    _logger.LogInformation("Product found: {ProductName} with ID: {ProductId}", 
                        productName, productId);
                    return productId;
                }
                else
                {
                    _logger.LogInformation("Product not found: {ProductName}", productName);
                    throw new KeyNotFoundException($"Product '{productName}' not found");
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while fetching product: {ProductName}", productName);
                throw new InvalidOperationException("An error occurred while accessing the database", ex);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException)
            {
                _logger.LogError(ex, "Unexpected error while fetching product: {ProductName}", productName);
                throw;
            }
        }

        /// <summary>
        /// Alternative method using stored procedure (even more secure)
        /// </summary>
        public async Task<int> GetProductByStoredProcedure(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ArgumentException("Product name cannot be empty", nameof(productName));
            }

            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured");
            }

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("GetProductByName", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add("@ProductName", SqlDbType.NVarChar, MaxProductNameLength).Value = productName;
                
                // Add output parameter for better error handling
                var returnValue = command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                returnValue.Direction = ParameterDirection.ReturnValue;

                await using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    return reader.GetInt32(0);
                }
                
                throw new KeyNotFoundException($"Product '{productName}' not found");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in stored procedure: {ProductName}", productName);
                throw new InvalidOperationException("An error occurred while accessing the database", ex);
            }
        }
    }
}
