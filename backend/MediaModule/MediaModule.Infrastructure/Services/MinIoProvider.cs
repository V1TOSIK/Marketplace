using MediaModule.Application.Interfaces;
using MediaModule.Domain.Entities;
using MediaModule.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace MediaModule.Infrastructure.Services
{
    public class MinIoProvider : IMediaProvider
    {
        private readonly IMinioClient _minioClient;
        private readonly MinIoOptions _minIoOptions;
        private readonly ILogger<MinIoProvider> _logger;
        private readonly string _bucketName;
        public MinIoProvider(IMinioClient minioClient,
            IOptions<MinIoOptions> options,
            ILogger<MinIoProvider> logger)
        {
            _minioClient = minioClient;
            _minIoOptions = options.Value;
            _bucketName = _minIoOptions.BucketName;
            _logger = logger;
            if (string.IsNullOrWhiteSpace(_minIoOptions.BucketName))
                throw new ArgumentException("Bucket name cannot be null or empty", nameof(_minIoOptions.BucketName));
        }
        private async Task EnsureBucketExistsAsync()
        {
            var args = new BucketExistsArgs()
                .WithBucket(_bucketName);
            if (!await CheckBucketExistAsync())
            {
                await CreateBucketAsync();
                _logger.LogInformation($"Bucket '{_bucketName}' created successfully.");
            }
            else
            {
                _logger.LogInformation($"Bucket '{_bucketName}' already exists.");
            }
        }

        private async Task<bool> CheckBucketExistAsync()
        {
            var args = new BucketExistsArgs()
                .WithBucket(_bucketName);

            return await _minioClient.BucketExistsAsync(args);
        }

        private async Task CreateBucketAsync()
        {
            var args = new MakeBucketArgs()
                .WithBucket(_bucketName);
            await _minioClient.MakeBucketAsync(args);
        }

        public async Task<string> AddMediaAsync(string objectName, string contentType, Stream stream)
        {
            try
            {
                await EnsureBucketExistsAsync();

                long size;
                if (stream.CanSeek)
                {
                    size = stream.Length;
                }
                else
                {
                    var temp = new MemoryStream();
                    await stream.CopyToAsync(temp);
                    temp.Seek(0, SeekOrigin.Begin);
                    stream = temp;
                    size = temp.Length;
                }

                var args = new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(size)
                    .WithContentType(contentType);

                await _minioClient.PutObjectAsync(args);

                var scheme = _minIoOptions.UseSSL ? "https" : "http";
                var url = $"{scheme}://{_minIoOptions.Endpoint}/{_bucketName}/{objectName}";
                return url;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Stream> GetMediaAsync(string objectName)
        {
            try
            {
                var memoryStream = new MemoryStream();
                var args = new GetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(async (stream) =>
                    {
                        await stream.CopyToAsync(memoryStream);
                    });
                await _minioClient.GetObjectAsync(args);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteMediaAsync(string objectName)
        {
            try
            {
                var args = new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName);

                await _minioClient.RemoveObjectAsync(args);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
