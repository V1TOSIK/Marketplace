using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class UserOperationException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public UserOperationException(string message)
            : base(message) { }

        public UserOperationException(string message, Exception innerException)
            : base(message, innerException) { }

        public UserOperationException()
            : base("User operation conflict occured.") { }

        public UserOperationException(Exception innerException)
            : base("User operation conflict occurred.", innerException) { }
    }
}
