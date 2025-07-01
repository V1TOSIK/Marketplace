using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class InvalidUserDataException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public InvalidUserDataException(string message)
            : base(message) { }
        public InvalidUserDataException(string message, Exception innerException)
            : base(message, innerException) { }
        public InvalidUserDataException()
            : base("Invalid user data provided.") { }
        public InvalidUserDataException(Exception innerException)
            : base("Invalid user data provided.", innerException) { }
    }
}
