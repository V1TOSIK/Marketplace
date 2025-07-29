using MediaModule.Application.Dtos.Requests;
using MediaModule.Application.Dtos.Responses;

namespace MediaModule.Application.Interfaces
{
    public interface IMediaService
    {
        Task<IEnumerable<string>> GetAllEntityMediaUrls(Guid entityId);
        Task<string> GetMainMediaUrl(Guid entityId);
        Task AddMedia(UploadMediaRequest media);
        Task SoftDeleteMedia(Guid mediaId);
        Task HardDeleteMedia(Guid mediaId);
    }
}
