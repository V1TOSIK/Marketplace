namespace SharedKernel.Pagination
{
    public class PaginationRequest
    {
        public PaginationRequest() { }
        public PaginationRequest(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
