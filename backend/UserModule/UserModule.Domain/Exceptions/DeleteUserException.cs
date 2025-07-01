using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class DeleteUserException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public DeleteUserException(string message)
            : base(message) { }
        public DeleteUserException(string message, Exception innerException)
            : base(message, innerException) { }
        public DeleteUserException()
            : base("An error occurred while deleting the user.") { }
        public DeleteUserException(Exception innerException)
            : base("An error occurred while deleting the user.", innerException) { }
    }
}
