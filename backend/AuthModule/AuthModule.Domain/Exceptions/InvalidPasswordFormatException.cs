using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class InvalidPasswordFormatException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public InvalidPasswordFormatException(string message)
            : base(message) { }

        public InvalidPasswordFormatException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidPasswordFormatException()
            : base("Password is not in a valid format.") { }

        public InvalidPasswordFormatException(Exception innerException)
            : base("Password is not in a valid format.", innerException) { }
    }
}
