using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class InvalidUserOperationException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public InvalidUserOperationException(string message)
            : base(message) { }
        public InvalidUserOperationException(string message, Exception innerException)
            : base(message, innerException) { }
        public InvalidUserOperationException()
            : base("Invalid user operation.") { }
        public InvalidUserOperationException(Exception innerException)
            : base("Invalid user operation.", innerException) { }
    }
}
