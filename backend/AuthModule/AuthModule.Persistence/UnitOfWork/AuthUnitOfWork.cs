using AuthModule.Application.Interfaces;
using SharedKernel.Interfaces;

namespace AuthModule.Persistence.UnitOfWork
{
    public class AuthUnitOfWork : IAuthUnitOfWork
    {
        private readonly IUnitOfWork<AuthDbContext> _unitOfWork;
        public AuthUnitOfWork(IUnitOfWork<AuthDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _unitOfWork.SaveChangesAsync(cancellationToken);

        public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken)
            => await _unitOfWork.ExecuteInTransactionAsync(action, cancellationToken);
    }
}
