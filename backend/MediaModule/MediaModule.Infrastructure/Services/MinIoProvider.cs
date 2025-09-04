using MediaModule.Application.Interfaces.Services;
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
        private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
        {
            if (await CheckBucketExistAsync(cancellationToken))
                _logger.LogInformation($"Bucket '{_bucketName}' already exists.");
            else
            {
                await CreateBucketAsync(cancellationToken);
                _logger.LogInformation($"Bucket '{_bucketName}' created successfully.");
            }
        }

        private async Task<bool> CheckBucketExistAsync(CancellationToken cancellationToken)
        {

            var args = new BucketExistsArgs()
                .WithBucket(_bucketName);
            return await _minioClient.BucketExistsAsync(args, cancellationToken);
        }

        private async Task CreateBucketAsync(CancellationToken cancellationToken)
        {
            var args = new MakeBucketArgs()
                .WithBucket(_bucketName);
            await _minioClient.MakeBucketAsync(args, cancellationToken);
        }

        public async Task<string> AddMediaAsync(string objectName, string contentType, Stream stream, CancellationToken cancellationToken)
        {
            try
            {
                await EnsureBucketExistsAsync(cancellationToken);

                long size;
                if (stream.CanSeek)
                {
                    size = stream.Length;
                }
                else
                {
                    var temp = new MemoryStream();
                    await stream.CopyToAsync(temp, cancellationToken);
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

                await _minioClient.PutObjectAsync(args, cancellationToken);

                var scheme = _minIoOptions.UseSSL ? "https" : "http";
                var url = $"{scheme}://{_minIoOptions.Endpoint}/{_bucketName}/{objectName}";
                return url;

            }
            catch (Exception ex)
            {
                throw new Exception("Exception: ", ex);
            }
        }

        public async Task<Stream> GetMediaAsync(string objectName, CancellationToken cancellationToken)
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
                await _minioClient.GetObjectAsync(args, cancellationToken);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: ", ex);
            }
        }

        public async Task DeleteMediaAsync(string objectName, CancellationToken cancellationToken)
        {
            try
            {
                var args = new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName);

                await _minioClient.RemoveObjectAsync(args, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: ", ex);
            }

        }
    }
}
