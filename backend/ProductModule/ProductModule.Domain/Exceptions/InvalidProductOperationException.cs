using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class InvalidProductOperationException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

        public InvalidProductOperationException(string message)
            : base(message) { }

        public InvalidProductOperationException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidProductOperationException()
            : base("Invalid product operation.") { }

        public InvalidProductOperationException(Exception innerException)
            : base("Invalid product operation.", innerException) { }
    }
}
