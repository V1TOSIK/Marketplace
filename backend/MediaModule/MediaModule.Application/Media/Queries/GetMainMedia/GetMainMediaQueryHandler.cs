using MediaModule.Application.Interfaces.Repositories;
using MediatR;
using SharedKernel.Queries;

namespace MediaModule.Application.Media.Queries.GetMainMedia
{
    public class GetMainMediaQueryHandler : IRequestHandler<GetMainMediaQuery, string>
    {
        private readonly IMediaRepository _mediaRepository;

        public GetMainMediaQueryHandler(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<string> Handle(GetMainMediaQuery query, CancellationToken cancellationToken)
        {
            var media = await _mediaRepository.GetMainMediaByEntityIdAsync(query.EntityId, cancellationToken);

            return media.Url;
        }
    }
}
