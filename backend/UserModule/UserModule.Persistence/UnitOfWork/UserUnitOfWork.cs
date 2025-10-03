using SharedKernel.UnitOfWork;
using UserModule.Application.Interfaces;

namespace UserModule.Persistence.UnitOfWork
{
    public class UserUnitOfWork : IUserUnitOfWork
    {
        private readonly IUnitOfWork<UserDbContext> _unitOfWork;
        public UserUnitOfWork(IUnitOfWork<UserDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _unitOfWork.SaveChangesAsync(cancellationToken);

        public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken)
            => await _unitOfWork.ExecuteInTransactionAsync(action, cancellationToken);
    }
}
