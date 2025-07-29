using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Application.Exceptions
{
    public class UserAlreadyRegisteredException :BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public UserAlreadyRegisteredException(string message)
            : base(message) { }

        public UserAlreadyRegisteredException(string message, Exception innerException)
            : base(message, innerException) { }

        public UserAlreadyRegisteredException()
            : base("User already registered.") { }

        public UserAlreadyRegisteredException(Exception innerException)
            : base("User already registered.", innerException) { }
    }
}
