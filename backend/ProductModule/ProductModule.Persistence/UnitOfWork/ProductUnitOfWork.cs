using ProductModule.Application.Interfaces;
using SharedKernel.Interfaces;

namespace ProductModule.Persistence.UnitOfWork
{
    public class ProductUnitOfWork : IProductUnitOfWork
    {
        private readonly IUnitOfWork<ProductDbContext> _unitOfWork;
        public ProductUnitOfWork(IUnitOfWork<ProductDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _unitOfWork.SaveChangesAsync(cancellationToken);

        public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken)
            => await _unitOfWork.ExecuteInTransactionAsync(action, cancellationToken);
    }
}
