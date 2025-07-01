using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Application.Exceptions
{
    public class UserNotFoundException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

        public UserNotFoundException(string message)
            : base(message) { }

        public UserNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        public UserNotFoundException()
            : base("User not found.") { }

        public UserNotFoundException(Exception innerException)
            : base("User not found.", innerException) { }
    }
}
