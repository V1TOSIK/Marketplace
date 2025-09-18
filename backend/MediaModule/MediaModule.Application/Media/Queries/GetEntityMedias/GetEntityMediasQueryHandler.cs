using MediaModule.Application.Interfaces.Repositories;
using MediatR;
using SharedKernel.Dtos;
using SharedKernel.Queries;

namespace MediaModule.Application.Media.Queries.GetEntityMedias
{
    public class GetEntityMediasQueryHandler : IRequestHandler<GetEntityMediasQuery, IEnumerable<MediaDto>>
    {
        private readonly IMediaRepository _mediaRepository;
        public GetEntityMediasQueryHandler(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<IEnumerable<MediaDto>> Handle(GetEntityMediasQuery query, CancellationToken cancellationToken)
        {
            var medias = await _mediaRepository.GetMediasByEntityIdAsync(query.EntityId, cancellationToken);

            return medias.Select(m => new MediaDto()
            {
                Id = m.Id,
                Url = m.Url,
                IsMain = m.IsMain
            }).ToList();
        }
    }
}
