using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class RefreshTokenOperationException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public RefreshTokenOperationException(string message)
            : base(message) { }

        public RefreshTokenOperationException(string message, Exception innerException)
            : base(message, innerException) { }

        public RefreshTokenOperationException()
            : base("Refresh token conflict occurred.") { }

        public RefreshTokenOperationException(Exception innerException)
            : base("Refresh token conflict occurred.", innerException) { }
    }
}
