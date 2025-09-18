using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class UserNotFoundException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
        public UserNotFoundException()
            : base("User not found.") { }
        public UserNotFoundException(string message)
            : base(message) { }
        public UserNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
        public UserNotFoundException(Exception innerException)
            : base("A user-related operation failed because the user was not found.", innerException) { }
    }
}
