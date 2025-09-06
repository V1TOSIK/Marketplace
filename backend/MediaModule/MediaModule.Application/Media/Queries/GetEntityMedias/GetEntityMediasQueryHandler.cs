using MediaModule.Application.Interfaces.Repositories;
using MediatR;
using SharedKernel.Queries;

namespace MediaModule.Application.Media.Queries.GetEntityMedias
{
    public class GetEntityMediasQueryHandler : IRequestHandler<GetEntityMediasQuery, IEnumerable<string>>
    {
        private readonly IMediaRepository _mediaRepository;
        public GetEntityMediasQueryHandler(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<IEnumerable<string>> Handle(GetEntityMediasQuery query, CancellationToken cancellationToken)
        {
            var medias = await _mediaRepository.GetMediasByEntityIdAsync(query.EntityId, cancellationToken);

            return medias.Select(m => m.Url).ToList();
        }
    }
}
