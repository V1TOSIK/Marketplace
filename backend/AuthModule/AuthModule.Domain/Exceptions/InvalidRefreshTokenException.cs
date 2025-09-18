using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class InvalidRefreshTokenException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public InvalidRefreshTokenException(string message)
             : base(message) { }
        public InvalidRefreshTokenException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidRefreshTokenException()
            : base("Invalid refresh token format.") { }

        public InvalidRefreshTokenException(Exception innerException)
            : base("Invalid refresh token format.", innerException) { }
    }
}
