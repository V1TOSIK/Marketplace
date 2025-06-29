using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Infrastructure.Exceptions
{
    public class NullablePasswordException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public NullablePasswordException(string message)
            : base(message) { }

        public NullablePasswordException(string message, Exception innerException)
            : base(message, innerException) { }

        public NullablePasswordException()
            : base("Password cannot be null or empty.") { }

        public NullablePasswordException(Exception innerException)
            : base("Password cannot be null or empty.", innerException) { }
    }
}
