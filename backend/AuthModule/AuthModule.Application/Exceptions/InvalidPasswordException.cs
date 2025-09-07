using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Application.Exceptions
{
    public class InvalidPasswordException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

        public InvalidPasswordException(string message)
            : base(message) { }

        public InvalidPasswordException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidPasswordException()
            : base("Password is incorrect.") { }

        public InvalidPasswordException(Exception innerException)
            : base("Password is incorrect.", innerException) { }
    }
}
