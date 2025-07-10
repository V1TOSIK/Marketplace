using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Application.Exceptions
{
    public class BannedUserException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

        public BannedUserException(string message)
            : base(message) { }

        public BannedUserException(string message, Exception innerException)
            : base(message, innerException) { }

        public BannedUserException()
            : base("User is banned") { }

        public BannedUserException(Exception innerException)
            : base("User is banned", innerException) { }
    }
}
