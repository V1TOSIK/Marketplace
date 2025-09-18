using SharedKernel.Exceptions;
using System.Net;

namespace MediaModule.Application.Exceptions
{
    public class NullableMediaException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public NullableMediaException(string message)
            : base(message) { }
        public NullableMediaException(string message, Exception innerException)
            : base(message, innerException) { }
        public NullableMediaException()
            : base("Media cannot be null.") { }
        public NullableMediaException(Exception innerException)
            : base("Media cannot be null.", innerException) { }
    }
}
