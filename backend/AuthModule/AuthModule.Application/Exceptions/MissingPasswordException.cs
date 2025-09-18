using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Application.Exceptions
{
    public class MissingPasswordException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

        public MissingPasswordException(string message)
            : base(message) { }

        public MissingPasswordException(string message, Exception innerException)
            : base(message, innerException) { }

        public MissingPasswordException()
            : base("Password is missed.") { }

        public MissingPasswordException(Exception innerException)
            : base("Password is missed.", innerException) { }
    }
}
