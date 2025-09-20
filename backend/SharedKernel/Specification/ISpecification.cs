using System.Linq.Expressions;

namespace SharedKernel.Specification
{
    public interface ISpecification<T>
    {
        List<Expression<Func<T, bool>>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }
        int PageNumber { get; }
        int PageSize { get; }
    }
}
