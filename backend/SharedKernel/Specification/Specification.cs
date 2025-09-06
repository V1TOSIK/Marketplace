using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;

namespace SharedKernel.Specification
{
    public class Specification<T> : ISpecification<T>
    {
        public List<Expression<Func<T, bool>>> Criteria { get; } = new();
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public bool IsPagingEnabled { get; private set; } = false;
        public int? PageNumber { get; private set; }
        public int? PageSize { get; private set; }

        public Specification<T> AddCriteria(Expression<Func<T, bool>> criterion)
        {
            Criteria.Add(criterion);
            return this;
        }

        public Specification<T> AddInclude(Expression<Func<T, object>> include)
        {
            Includes.Add(include);
            return this;
        }

        public Specification<T> ApplyOrderByProperty(string propertyName, bool descending = false)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression property = param;

            foreach (var prop in propertyName.Split('.'))
            {
                property = Expression.PropertyOrField(property, prop);
            }

            var converted = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(converted, param);

            if (descending)
                OrderByDescending = lambda;
            else
                OrderBy = lambda;

            return this;
        }

        public Specification<T> ApplyPaging(int pageNumber, int pageSize)
        {
            IsPagingEnabled = true;
            PageNumber = pageNumber;
            PageSize = pageSize;
            return this;
        }
    }
}
