using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class InvalidEmailFormatException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public InvalidEmailFormatException(string message)
            : base(message) { }

        public InvalidEmailFormatException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidEmailFormatException()
            : base($"Email is not in a valid format.") { }

        public InvalidEmailFormatException(Exception innerException)
            : base($"Email is not in a valid format.", innerException) { }
    }
}
