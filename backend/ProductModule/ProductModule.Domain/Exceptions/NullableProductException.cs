using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class NullableProductException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public NullableProductException(string message)
            : base(message) { }

        public NullableProductException(string message, Exception innerException)
            : base(message, innerException) { }

        public NullableProductException()
            : base("Product cannot be null.") { }

        public NullableProductException(Exception innerException)
            : base("Product cannot be null.", innerException) { }
    }
}
