using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Application.Exceptions
{
    public class IncorrectCredentialsException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public IncorrectCredentialsException(string message)
            : base(message) { }

        public IncorrectCredentialsException(string message, Exception innerException)
            : base(message, innerException) { }

        public IncorrectCredentialsException()
            : base("Password incorect.") { }

        public IncorrectCredentialsException(Exception innerException)
            : base("Password incorect.", innerException) { }
    }
}
