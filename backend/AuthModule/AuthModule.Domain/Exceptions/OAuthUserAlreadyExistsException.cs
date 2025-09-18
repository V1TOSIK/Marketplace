using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class OAuthUserAlreadyExistsException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

        public OAuthUserAlreadyExistsException(string message)
            : base(message) { }

        public OAuthUserAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException) { }

        public OAuthUserAlreadyExistsException()
            : base("User with OAuth data already exists.") { }

        public OAuthUserAlreadyExistsException(Exception innerException)
            : base("User with OAuth data already exists.", innerException) { }
    }
}
