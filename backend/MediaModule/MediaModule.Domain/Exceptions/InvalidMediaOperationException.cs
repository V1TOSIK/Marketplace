using SharedKernel.Exceptions;
using System.Net;

namespace MediaModule.Domain.Exceptions
{
    public class InvalidMediaOperationException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public InvalidMediaOperationException(string message)
            : base(message) { }

        public InvalidMediaOperationException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidMediaOperationException()
            : base("Media conflict occurred.") { }

        public InvalidMediaOperationException(Exception innerException)
            : base("Media conflict occurred.", innerException) { }
    }
}
