using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class EmailAlreadyExistsException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

        public EmailAlreadyExistsException(string message)
            : base(message) { }

        public EmailAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException) { }

        public EmailAlreadyExistsException()
            : base("User with email already exists.") { }

        public EmailAlreadyExistsException(Exception innerException)
            : base("User with email already exists.", innerException) { }
    }
}
