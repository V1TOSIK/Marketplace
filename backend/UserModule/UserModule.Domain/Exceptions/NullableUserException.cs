using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class NullableUserException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public NullableUserException()
            : base($"User cannot be null.") { }
        public NullableUserException(string message)
            : base(message) { }
        public NullableUserException(string message, Exception innerException)
            : base(message, innerException) { }
        public NullableUserException(Exception innerException)
            : base("A user-related operation failed due to a null user.", innerException)
        {
        }
    }
}
