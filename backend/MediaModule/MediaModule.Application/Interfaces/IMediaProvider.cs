using MediaModule.Domain.Entities;

namespace MediaModule.Application.Interfaces
{
    public interface IMediaProvider
    {
        public Task<string> AddMediaAsync(string objectName, string contentType, Stream stream);
        public Task<Stream> GetMediaAsync(string objectName);
        public Task DeleteMediaAsync(string objectName);
    }
}
