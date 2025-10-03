using MediaModule.Application.Interfaces;
using SharedKernel.UnitOfWork;

namespace MediaModule.Persistence.UnitOfWork
{
    public class MediaUnitOfWork : IMediaUnitOfWork
    {
        private readonly IUnitOfWork<MediaDbContext> _unitOfWork;
        public MediaUnitOfWork(IUnitOfWork<MediaDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _unitOfWork.SaveChangesAsync(cancellationToken);

        public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken)
            => await _unitOfWork.ExecuteInTransactionAsync(action, cancellationToken);
    }
}
