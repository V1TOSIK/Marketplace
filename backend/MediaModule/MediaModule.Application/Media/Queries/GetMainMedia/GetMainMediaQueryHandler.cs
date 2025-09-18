using MediaModule.Application.Interfaces.Repositories;
using MediatR;
using SharedKernel.Dtos;
using SharedKernel.Queries;

namespace MediaModule.Application.Media.Queries.GetMainMedia
{
    public class GetMainMediaQueryHandler : IRequestHandler<GetMainMediaQuery, MediaDto>
    {
        private readonly IMediaRepository _mediaRepository;

        public GetMainMediaQueryHandler(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<MediaDto> Handle(GetMainMediaQuery query, CancellationToken cancellationToken)
        {
            var media = await _mediaRepository.GetMainMediaByEntityIdAsync(query.EntityId, cancellationToken);

            return new MediaDto
            {
                Id = media.Id,
                Url = media.Url,
                IsMain = media.IsMain
            };
        }
    }
}
