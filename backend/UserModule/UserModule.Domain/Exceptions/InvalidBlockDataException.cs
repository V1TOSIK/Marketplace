using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class InvalidBlockDataException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public InvalidBlockDataException(string message)
            : base(message) { }
        public InvalidBlockDataException(string message, Exception innerException)
            : base(message, innerException) { }
        public InvalidBlockDataException()
            : base("Invalid block data provided.") { }
        public InvalidBlockDataException(Exception innerException)
            : base("Invalid block data provided.", innerException) { }
    }
}
