using Minio.DataModel;

namespace PhiThanh.Core.Services
{
    public interface IFileStorage
    {
        Task<string> UploadAsync(Stream file, string filePath, string mimeType = "application/octet-stream", bool isPublic = true,
            int secondExpiry = 86400, string source = "from-api-upload", Dictionary<string, string>? headers = null,
            bool returnUrl = true);
        Task<bool> IsExistsAsync(string filePath, bool isPublic = true);
        Task<string> GetUrlAsync(string filePath, bool isPublic = true, int secondExpiry = 86400);
        Task<ObjectStat?> GetObjectAsync(string filePath, bool isPublic = true);
        Task<bool> RemoveAsync(string source, bool isPublic = true);
        Task<Stream?> DownloadAsync(string filePath, bool isPublic = true);
        Task<List<Item>> GetListFilesAsync(string directoryPath, bool isPublic = true);
        Task<bool> CopyAsync(string source, string target, bool isPublic = true);
    }
}
