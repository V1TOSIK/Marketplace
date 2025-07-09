using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Application.Exceptions
{
    public class BadRequestException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public BadRequestException(string message)
            : base(message) { }
        public BadRequestException(string message, Exception innerException)
            : base(message, innerException) { }
        public BadRequestException()
            : base("Bad request data") { }
        public BadRequestException(Exception innerException)
            : base("Bad request data", innerException) { }
    }
}
