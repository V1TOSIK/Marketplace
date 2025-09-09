using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class CategoryNotFoundException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

        public CategoryNotFoundException(string message)
            : base(message) { }

        public CategoryNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        public CategoryNotFoundException()
            : base("Product not found.") { }

        public CategoryNotFoundException(Exception innerException)
            : base("Product not found.", innerException) { }
    }
}
