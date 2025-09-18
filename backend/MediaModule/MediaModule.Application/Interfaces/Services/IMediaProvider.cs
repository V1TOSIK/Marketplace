namespace MediaModule.Application.Interfaces.Services
{
    public interface IMediaProvider
    {
        public Task<string> AddMediaAsync(string objectName, string contentType, Stream stream, CancellationToken cancellationToken);
        public Task<Stream> GetMediaAsync(string objectName, CancellationToken cancellationToken);
        public Task DeleteMediaAsync(string objectName, CancellationToken cancellationToken);
    }
}
