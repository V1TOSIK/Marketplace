using MediaModule.Application.Interfaces.Repositories;
using MediatR;
using SharedKernel.Dtos;
using SharedKernel.Queries;

namespace MediaModule.Application.Media.Queries.GetMainMedias
{
    public class GetMainMediasQueryHandler : IRequestHandler<GetMainMediasQuery, Dictionary<Guid, MediaDto>>
    {
        private readonly IMediaRepository _mediaRepository;

        public GetMainMediasQueryHandler(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<Dictionary<Guid, MediaDto>> Handle(GetMainMediasQuery query, CancellationToken cancellationToken)
        {
            return await _mediaRepository
                .GetMainMediaByEntityIdsAsync(query.EntityIds, cancellationToken);
        }
    }
}
