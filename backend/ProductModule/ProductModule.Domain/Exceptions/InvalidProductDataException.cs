using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class InvalidProductDataException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public InvalidProductDataException(string message)
            : base(message) { }

        public InvalidProductDataException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidProductDataException()
            : base("Invalid product data.") { }

        public InvalidProductDataException(Exception innerException)
            : base("Invalid product data.", innerException) { }
    }
}
