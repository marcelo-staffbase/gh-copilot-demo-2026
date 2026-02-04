using UnsecureApp.Services;

namespace UnsecureApp.Mocks
{
    public class MockFileService : IFileService
    {
        private readonly Dictionary<string, string> _fileContents;
        private readonly HashSet<string> _lockedFiles;

        public MockFileService()
        {
            _fileContents = new Dictionary<string, string>();
            _lockedFiles = new HashSet<string>();
        }

        public void AddFile(string filePath, string content)
        {
            _fileContents[filePath] = content;
        }

        public void RemoveFile(string filePath)
        {
            _fileContents.Remove(filePath);
        }

        public void LockFile(string filePath)
        {
            _lockedFiles.Add(filePath);
        }

        public void UnlockFile(string filePath)
        {
            _lockedFiles.Remove(filePath);
        }

        public string? ReadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (_lockedFiles.Contains(filePath))
            {
                throw new IOException($"File is locked: {filePath}");
            }

            if (!_fileContents.ContainsKey(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            string content = _fileContents[filePath];
            
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            // Simulate the 1024 byte buffer behavior
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
            int bytesToRead = Math.Min(bytes.Length, 1024);
            byte[] buffer = new byte[bytesToRead];
            Array.Copy(bytes, buffer, bytesToRead);

            return System.Text.Encoding.UTF8.GetString(buffer);
        }
    }
}
