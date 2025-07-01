using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class NullableUserException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public NullableUserException(string message)
            : base(message) { }

        public NullableUserException(string message, Exception innerException)
            : base(message, innerException) { }

        public NullableUserException()
            : base("User cannot be null.") { }

        public NullableUserException(Exception innerException)
            : base("User cannot be null.", innerException) { }
    }
}
