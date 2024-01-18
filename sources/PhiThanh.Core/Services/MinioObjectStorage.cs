using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Tags;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace PhiThanh.Core.Services
{
    public class MinioOptions
    {
        public string Endpoint { get; set; }
        public string BaseUrl { get; set; }
        public string Region { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string PublicBucketName { get; set; }
        public string PrivateBucketName { get; set; }
        public bool SslEnabled { get; set; }
    }

    public class MinioObjectStorage : IFileStorage, IDisposable
    {
        private readonly IMinioClient _minio;
        private readonly MinioOptions _options;

        private string Endpoint()
        {
            if (_options.SslEnabled)
            {
                return $"https://{_options.BaseUrl}";
            }
            return $"http://{_options.BaseUrl}";
        }

        public MinioObjectStorage(MinioOptions options)
        {
            _options = options;
            _minio = new MinioClient()
                    .WithEndpoint(_options.Endpoint)
                    .WithRegion(_options.Region)
                    .WithSSL(_options.SslEnabled)
                    .WithCredentials(_options.AccessKey, _options.SecretKey)
                    .Build();
        }

        public async Task<bool> RemoveAsync(string source, bool isPublic = true)
        {
            try
            {
                var canRemove = false;
                var filePaths = new List<string>();

                var sourceItem = await GetObjectAsync(source, isPublic);
                if (sourceItem == null)
                {
                    var listFilesInSource = await GetListFilesAsync(source, isPublic);
                    if (listFilesInSource.Count > 0)
                    {
                        canRemove = true;
                        filePaths.AddRange(listFilesInSource.Select(s => s.Key));
                    }
                }
                else
                {
                    filePaths.Add(sourceItem.ObjectName);
                    canRemove = true;
                }

                if (canRemove)
                {
                    foreach (var file in filePaths)
                    {
                        RemoveObjectArgs rmArgs = new RemoveObjectArgs()
                       .WithBucket(GetBucketName(isPublic))
                       .WithObject(file);

                        await _minio.RemoveObjectAsync(rmArgs);
                    }
                }

                return canRemove;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Stream?> DownloadAsync(string filePath, bool isPublic = true)
        {
            if (await IsExistsAsync(filePath, isPublic))
            {
                var result = new MemoryStream();
                GetObjectArgs getObjectArgs = new GetObjectArgs()
                    .WithBucket(GetBucketName(isPublic))
                    .WithObject(filePath)
                    .WithCallbackStream((stream) => stream.CopyTo(result));

                _ = await _minio.GetObjectAsync(getObjectArgs);

                if (result.Length > 0)
                {
                    result.Seek(0, SeekOrigin.Begin);
                    return result;
                }
            }

            return null;
        }

        public async Task<ObjectStat?> GetObjectAsync(string filePath, bool isPublic = true)
        {
            if (await IsExistsAsync(filePath, isPublic))
            {
                GetObjectArgs getObjectArgs = new GetObjectArgs()
                    .WithBucket(GetBucketName(isPublic))
                    .WithObject(filePath)
                    .WithCallbackStream((_) => { });

                return await _minio.GetObjectAsync(getObjectArgs);
            }

            return null;
        }

        public async Task<string> UploadAsync(Stream file, string filePath, string mimeType = "application/octet-stream",
            bool isPublic = true, int secondExpiry = 86400, string source = "from-api-upload",
            Dictionary<string, string>? headers = null, bool returnUrl = true)
        {
            if (filePath != null)
            {
                filePath = filePath.Replace("\\", "/");
                var putObjectArgs = new PutObjectArgs()
                           .WithBucket(GetBucketName(isPublic))
                           .WithObject(filePath)
                           .WithStreamData(file)
                           .WithObjectSize(file.Length)
                           .WithTagging(new Tagging(new Dictionary<string, string> {
                       { "source", source},
                       { "public", isPublic.ToString()}
                           }, false))
                           .WithContentType(mimeType);

                if (headers != null)
                {
                    putObjectArgs = putObjectArgs.WithHeaders(headers);
                }

                _ = await _minio.PutObjectAsync(putObjectArgs);

                if (returnUrl)
                {
                    return await GetUrlAsync(filePath, isPublic, secondExpiry);
                }
            }
            return string.Empty;
        }

        public async Task<bool> IsExistsAsync(string filePath, bool isPublic = true)
        {
            var result = true;
            try
            {
                StatObjectArgs statObjectArgs = new StatObjectArgs()
                                           .WithBucket(GetBucketName(isPublic))
                                           .WithObject(filePath);

                _ = await _minio.StatObjectAsync(statObjectArgs);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public async Task<List<Item>> GetListFilesAsync(string directoryPath, bool isPublic = true)
        {
            var result = new List<Item>();
            try
            {
                ListObjectsArgs args = new ListObjectsArgs()
                                        .WithBucket(GetBucketName(isPublic))
                                        .WithPrefix(directoryPath)
                                        .WithRecursive(true);

                var files = _minio.ListObjectsAsync(args)
                    .ToList()
                    .ToTask();

                result.AddRange(await files);
                return result;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<bool> CopyAsync(string source, string target, bool isPublic = true)
        {
            try
            {
                var canCopy = false;
                var filePaths = new List<string>();

                var sourceItem = await GetObjectAsync(source, isPublic);
                if (sourceItem == null)
                {
                    var listFilesInSource = await GetListFilesAsync(source, isPublic);
                    if (listFilesInSource.Count > 0)
                    {
                        canCopy = true;
                        filePaths.AddRange(listFilesInSource.Select(s => s.Key));
                    }
                }
                else
                {
                    filePaths.Add(sourceItem.ObjectName);
                    canCopy = true;
                }

                if (canCopy)
                {
                    foreach (var file in filePaths)
                    {
                        CopySourceObjectArgs csArgs = new CopySourceObjectArgs()
                                                .WithBucket(GetBucketName(isPublic))
                                                .WithObject(file);

                        string fileNameTarget = $"{target}/{file.Replace(source, "").TrimStart('/')}";
                        CopyObjectArgs args = new CopyObjectArgs()
                                                .WithBucket(GetBucketName(isPublic))
                                                .WithObject(fileNameTarget)
                                                .WithCopyObjectSource(csArgs);

                        await _minio.CopyObjectAsync(args);
                    }
                }

                return canCopy;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> GetUrlAsync(string filePath, bool isPublic = true, int secondExpiry = 86400)
        {
            filePath = filePath.Replace("\\", "/");
            if (await IsExistsAsync(filePath, isPublic))
            {
                PresignedGetObjectArgs args = new PresignedGetObjectArgs()
                                         .WithBucket(GetBucketName(isPublic))
                                         .WithObject(filePath);

                if (!isPublic)
                {
                    args = args.WithExpiry(secondExpiry);
                    string url = await _minio.PresignedGetObjectAsync(args);
                    return url;
                }
                else
                {
                    return $"{Endpoint()}/{GetBucketName(true)}/{filePath}";
                }
            }
            return string.Empty;
        }

        private string GetBucketName(bool isPublic)
        {
            return isPublic
                ? _options.PublicBucketName
                : _options.PrivateBucketName;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                _minio.Dispose();
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
