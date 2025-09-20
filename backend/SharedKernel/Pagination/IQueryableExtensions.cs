using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Pagination
{
    public static class IQueryableExtensions
    {
        public static async Task<PaginationResponse<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginationResponse<T>(items, totalCount, pageNumber, pageSize);
        }
    }
}
