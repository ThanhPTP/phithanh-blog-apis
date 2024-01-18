using FFMpegCore;
using HeyRed.Mime;
using Microsoft.AspNetCore.Http;
using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.Core.Services;
using PhiThanh.Core.Utils;
using PhiThanh.Resources;
using Serilog;
using SkiaSharp;

namespace PhiThanh.Modules.FileModules
{
    public class UploadFilesRequest : BaseQuery<UploadFileResponse>
    {
        public List<IFormFile> Files { get; set; } = [];
        public List<int> ThumbSizes { get; set; } = [];

        public static async ValueTask<UploadFilesRequest?> BindAsync(HttpContext context)
        {
            var form = await context.Request.ReadFormAsync();
            var thumbSizes = form["ThumbSizes"];
            return new UploadFilesRequest
            {
                Files = [.. form.Files],
                ThumbSizes = thumbSizes.Select(s => int.Parse(s!)).ToList(),
            };
        }
    }

    public class UploadFileResponse
    {
        public List<string> Paths { get; set; } = [];
    }

    public class UploadFilesProfileMapper : AutoMapper.Profile
    {
        public UploadFilesProfileMapper()
        {
        }
    }

    public class UploadFilesValidator : BaseValidator<UploadFilesRequest>
    {
        public UploadFilesValidator()
        {
        }
    }

    public class UploadFilesHandler(IFileStorage fileStorage, ILogger logger) : IQueryHandler<UploadFilesRequest, UploadFileResponse>
    {
        private readonly IFileStorage _fileStorage = fileStorage;
        private readonly ILogger _logger = logger;

        public async Task<ApiResponse<UploadFileResponse>> Handle(UploadFilesRequest request, CancellationToken cancellationToken)
        {
            var response = new UploadFileResponse
            {
                Paths = []
            };

            try
            {
                string folderPattern = $"{HttpContextUtils.Identity?.UserName()}/{DateTime.UtcNow:dd-MM-yyyy}";
                foreach (var formFile in request.Files)
                {
                    if (formFile.Length > 0)
                    {
                        await using var stream = new MemoryStream();
                        await formFile.CopyToAsync(stream, cancellationToken);
                        stream.Seek(0, SeekOrigin.Begin);

                        string extension = Path.GetExtension(formFile.FileName).ToLower();
                        if (Constants.ExtensionsVideo.Contains(extension))
                        {
                            if (request.ThumbSizes.Count > 0)
                            {
                                await HandleVideoThumbnail(folderPattern, stream, request.ThumbSizes, formFile.FileName);
                            }

                            stream.Seek(0, SeekOrigin.Begin);
                            response.Paths.Add(await _fileStorage.UploadAsync(stream, $"{folderPattern}/_origin_/{formFile.FileName}",
                                formFile.ContentType));
                        }
                        else if (Constants.ExtensionsImages.Contains(extension))
                        {
                            if (extension != ".svg" && request.ThumbSizes.Count > 0)
                            {
                                await HandleImageThumbnail(folderPattern, stream, request.ThumbSizes, formFile.FileName);
                            }

                            stream.Seek(0, SeekOrigin.Begin);
                            response.Paths.Add(await _fileStorage.UploadAsync(stream, $"{folderPattern}/_origin_/{formFile.FileName}",
                                formFile.ContentType));
                        }
                        else
                        {
                            response.Paths.Add(await _fileStorage.UploadAsync(stream, $"{folderPattern}/{formFile.FileName}",
                                formFile.ContentType));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                ExceptionUtils.ThrowValidation(nameof(request.Files),
                    nameof(ValidationMessage.ERR_012_UPLOAD_FILE_ERROR),
                    ValidationMessage.ERR_012_UPLOAD_FILE_ERROR);
            }

            return new ApiResponse<UploadFileResponse>(response);
        }

        private async Task HandleVideoThumbnail(string folderPattern, MemoryStream stream, List<int> thumbSizes, string fileName)
        {
            var folderTempPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{Guid.NewGuid()}");
            if (!Directory.Exists(folderTempPath))
            {
                Directory.CreateDirectory(folderTempPath);
            }

            try
            {
                var inputFilePath = Path.Combine(folderTempPath, fileName);
                await using (var videoStream = File.Create(inputFilePath))
                {
                    await stream.CopyToAsync(videoStream);
                }

                string outputFile = Path.Combine(folderTempPath, $"{Guid.NewGuid()}.png");
                _ = FFMpeg.Snapshot(inputFilePath, outputFile,
                    captureTime: TimeSpan.FromSeconds(1));

                await using var outputStream = new MemoryStream(File.ReadAllBytes(outputFile));
                foreach (var thumbSize in thumbSizes)
                {
                    outputStream.Seek(0, SeekOrigin.Begin);
                    await using var videoStream = new MemoryStream();
                    outputStream.CopyTo(videoStream);
                    videoStream.Seek(0, SeekOrigin.Begin);

                    var skBitmap = SKBitmap.Decode(videoStream);

                    int srcWidth = skBitmap.Width;
                    int dstHeight = skBitmap.Height;
                    int dstWidth = thumbSize;

                    double ratio = srcWidth / (double)dstWidth;
                    dstHeight = (int)(dstHeight / ratio);

                    var dstInfo = new SKImageInfo(dstWidth, dstHeight);
                    skBitmap = skBitmap.Resize(dstInfo, SKFilterQuality.Medium);

                    using var image = SKImage.FromBitmap(skBitmap);
                    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                    await using var resultOutputStream = new MemoryStream();
                    data.SaveTo(resultOutputStream);
                    resultOutputStream.Seek(0, SeekOrigin.Begin);
                    var thumbUrl = await _fileStorage.UploadAsync(resultOutputStream, $"{folderPattern}/thumbs/{thumbSize}/{fileName}", "image/png");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                ExceptionUtils.ThrowValidation(nameof(fileName),
                    nameof(ValidationMessage.ERR_012_UPLOAD_FILE_ERROR),
                    ValidationMessage.ERR_012_UPLOAD_FILE_ERROR);
            }
            finally
            {
                Directory.Delete(folderTempPath, true);
            }
        }

        private async Task HandleImageThumbnail(string folderPattern, MemoryStream stream, List<int> thumbSizes, string fileName)
        {
            try
            {
                foreach (var thumbSize in thumbSizes)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    await using var streamImage = new MemoryStream();
                    stream.CopyTo(streamImage);
                    streamImage.Seek(0, SeekOrigin.Begin);
                    var skBitmap = SKBitmap.Decode(streamImage);

                    int srcWidth = skBitmap.Width;

                    int dstHeight = skBitmap.Height;
                    int dstWidth = thumbSize;

                    double ratio = srcWidth / (double)dstWidth;
                    dstHeight = (int)(dstHeight / ratio);

                    var dstInfo = new SKImageInfo(dstWidth, dstHeight);
                    skBitmap = skBitmap.Resize(dstInfo, SKFilterQuality.Medium);

                    using var image = SKImage.FromBitmap(skBitmap);
                    using var data = image.Encode(ConvertSKImageFormat(Path.GetExtension(fileName)), 100);
                    await using var resultOutputStream = new MemoryStream();
                    data.SaveTo(resultOutputStream);
                    resultOutputStream.Seek(0, SeekOrigin.Begin);
                    var thumbUrl = await _fileStorage.UploadAsync(resultOutputStream, $"{folderPattern}/_thumbs_/{thumbSize}/{fileName}", MimeTypesMap.GetMimeType(fileName));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                ExceptionUtils.ThrowValidation(nameof(folderPattern),
                    nameof(ValidationMessage.ERR_012_UPLOAD_FILE_ERROR),
                    ValidationMessage.ERR_012_UPLOAD_FILE_ERROR);
            }
        }

        private static SKEncodedImageFormat ConvertSKImageFormat(string extensions)
        {
            return extensions switch
            {
                ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,
                _ => SKEncodedImageFormat.Png,
            };
        }
    }
}
