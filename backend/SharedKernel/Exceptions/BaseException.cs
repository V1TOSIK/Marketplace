using System.Net;

namespace SharedKernel.Exceptions
{
    public class BaseException : Exception
    {
        public virtual HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;

        public BaseException(string message) : base(message) { }

        public BaseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
