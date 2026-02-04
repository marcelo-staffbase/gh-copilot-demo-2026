using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using UnsecureApp.Services;

namespace UnsecureApp.Controllers
{
    /// <summary>
    /// WARNING: This controller contains intentional security vulnerabilities for demonstration purposes.
    /// DO NOT USE IN PRODUCTION.
    /// See SecuredController.cs for the secure implementation.
    /// 
    /// Known vulnerabilities:
    /// 1. SQL Injection in GetProduct method
    /// 2. Path Traversal in ReadFile method
    /// 3. Missing input validation
    /// 4. Poor error handling
    /// 5. Hardcoded empty connection string
    /// </summary>
    public class MyController
    {
        private readonly IFileService _fileService;

        public MyController(IFileService? fileService = null)
        {
            _fileService = fileService ?? new FileService();
        }

        public string? ReadFile(string userInput)
        {
            return _fileService.ReadFile(userInput);
        }

        public int GetProduct(string productName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand()
                {
                    CommandText = "SELECT ProductId FROM Products WHERE ProductName = '" + productName + "'",
                    CommandType = CommandType.Text,
                };

                SqlDataReader reader = sqlCommand.ExecuteReader();
                return reader.GetInt32(0); 
            }
        }

        public void GetObject()
        {
            try
            {
                object? o = null;
                o?.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        
        }

        private string connectionString = "";
    }
}