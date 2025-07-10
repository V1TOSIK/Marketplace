using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class RefreshTokenNotFoundException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

        public RefreshTokenNotFoundException(string message)
            : base(message) { }

        public RefreshTokenNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        public RefreshTokenNotFoundException()
            : base("RefreshToken not found.") { }

        public RefreshTokenNotFoundException(Exception innerException)
            : base("RefreshToken not found.", innerException) { }
    }
}
