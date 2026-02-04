using System.Text;

namespace UnsecureApp.Services
{
    public class FileService : IFileService
    {
        public string? ReadFile(string filePath)
        {
            using (FileStream fs = File.Open(filePath, FileMode.Open))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                while (fs.Read(b, 0, b.Length) > 0)
                {
                    return temp.GetString(b);
                }
            }

            return null;
        }
    }
}
