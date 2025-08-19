using MediaModule.Application.Dtos.Requests;
using MediaModule.Application.Dtos.Responses;

namespace MediaModule.Application.Interfaces
{
    public interface IMediaService
    {
        Task<IEnumerable<MediaResponse>> GetAllEntityMediaUrls(Guid entityId, CancellationToken cancellationToken);
        Task AddMedia(UploadMediaRequest media, CancellationToken cancellationToken);
        Task SoftDeleteMedia(Guid mediaId, CancellationToken cancellationToken);
        Task HardDeleteMedia(Guid mediaId, CancellationToken cancellationToken);
    }
}
