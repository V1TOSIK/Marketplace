using MediaModule.Application.Interfaces.Repositories;
using MediatR;
using SharedKernel.Queries;

namespace MediaModule.Application.Media.Queries.GetMainMedias
{
    public class GetMainMediasQueryHandler : IRequestHandler<GetMainMediasQuery, Dictionary<Guid, string>>
    {
        private readonly IMediaRepository _mediaRepository;

        public GetMainMediasQueryHandler(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<Dictionary<Guid, string>> Handle(GetMainMediasQuery query, CancellationToken cancellationToken)
        {
            return await _mediaRepository
                .GetMainMediaUrlByEntityIdsAsync(query.EntityIds, cancellationToken);
        }
    }
}
