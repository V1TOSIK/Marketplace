using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class ProductNotFoundException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

        public ProductNotFoundException(string message)
            : base(message) { }

        public ProductNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        public ProductNotFoundException()
            : base("Product not found.") { }

        public ProductNotFoundException(Exception innerException)
            : base("Product not found.", innerException) { }
    }
}
