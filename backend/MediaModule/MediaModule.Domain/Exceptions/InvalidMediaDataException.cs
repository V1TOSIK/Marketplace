using SharedKernel.Exceptions;
using System.Net;

namespace MediaModule.Domain.Exceptions
{
    public class InvalidMediaDataException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public InvalidMediaDataException(string message)
            : base(message) { }
        public InvalidMediaDataException(string message, Exception innerException)
            : base(message, innerException) { }
        public InvalidMediaDataException()
            : base("Invalid media data provided.") { }
        public InvalidMediaDataException(Exception innerException)
            : base("Invalid media data provided.", innerException) { }
    }
}
