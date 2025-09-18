using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Application.Exceptions
{
    class UnauthorizedChangePasswordException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

        public UnauthorizedChangePasswordException(string message)
            : base(message) { }

        public UnauthorizedChangePasswordException(string message, Exception innerException)
            : base(message, innerException) { }

        public UnauthorizedChangePasswordException()
            : base("User are not authorized to change this user's password.") { }

        public UnauthorizedChangePasswordException(Exception innerException)
            : base("User are not authorized to change this user's password.", innerException) { }
    }
}
