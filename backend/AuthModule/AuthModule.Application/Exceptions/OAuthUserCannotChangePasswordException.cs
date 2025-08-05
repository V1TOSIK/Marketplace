using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Application.Exceptions
{
    public class OAuthUserCannotChangePasswordException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

        public OAuthUserCannotChangePasswordException(string message)
            : base(message) { }

        public OAuthUserCannotChangePasswordException(string message, Exception innerException)
            : base(message, innerException) { }

        public OAuthUserCannotChangePasswordException()
            : base("User cannot change password.") { }

        public OAuthUserCannotChangePasswordException(Exception innerException)
            : base("User cannot change password.", innerException) { }
    }
}
