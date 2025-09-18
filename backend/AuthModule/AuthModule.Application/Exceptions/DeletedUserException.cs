using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Application.Exceptions
{
    public class DeletedUserException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

        public DeletedUserException(string message)
            : base(message) { }

        public DeletedUserException(string message, Exception innerException)
            : base(message, innerException) { }

        public DeletedUserException()
            : base("User is deleted") { }

        public DeletedUserException(Exception innerException)
            : base("User is deleted", innerException) { }
    }
}
