namespace CsvToVcfConverter.Services
{
    public class FileStorageService
    {
        private readonly string _tempDirectory;

        public FileStorageService()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "CsvToVcf");
            Directory.CreateDirectory(_tempDirectory);
        }

        public string SaveTempFile(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_tempDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return filePath;
        }

        public void DeleteTempFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // Ignore deletion errors
            }
        }

        public Stream GetFileStream(string filePath)
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
    }
}