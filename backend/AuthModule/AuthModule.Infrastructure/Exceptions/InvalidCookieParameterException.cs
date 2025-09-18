using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Application.Exceptions
{
    public class InvalidCookieParameterException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public InvalidCookieParameterException(string message)
            : base(message) { }

        public InvalidCookieParameterException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidCookieParameterException()
            : base("Some cookie parameter is invalid.") { }

        public InvalidCookieParameterException(Exception innerException)
            : base("Some cookie parameter is invalid.", innerException) { }
    }
}
