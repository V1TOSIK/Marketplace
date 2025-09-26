using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Application.Exceptions
{
    public class AddPasswordException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

        public AddPasswordException(string message)
            : base(message) { }

        public AddPasswordException(string message, Exception innerException)
            : base(message, innerException) { }

        public AddPasswordException()
            : base("Cannot add password.") { }

        public AddPasswordException(Exception innerException)
            : base("Cannot add password.", innerException) { }
    }
}
