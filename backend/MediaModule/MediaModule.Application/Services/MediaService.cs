using MediaModule.Application.Interfaces.Services;

namespace MediaModule.Application.Services
{
    public class MediaService : IMediaService
    {
        public string CombineFileName(string entityId, string fileName)
        {
            return $"{entityId}/{fileName}";
        }
    }
}
