using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnsecureApp.Services;

namespace UnsecureApp.Controllers
{
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