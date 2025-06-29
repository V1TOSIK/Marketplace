using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class MissingAuthCredentialException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public MissingAuthCredentialException(string message)
            : base(message) { }

        public MissingAuthCredentialException(string message, Exception innerException)
            : base(message, innerException) { }

        public MissingAuthCredentialException()
            : base("Email or phone number must be not empty") { }

        public MissingAuthCredentialException(Exception innerException)
            : base("Email or phone number must be not empty", innerException) { }
    }
}
