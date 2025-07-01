using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class UserExistException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public UserExistException()
            : base("User already exists.") { }
        public UserExistException(string message)
            : base(message) { }
        public UserExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public UserExistException(Exception innerException)
            : base("A user-related operation failed due to an existing user.", innerException) { }
    }
}
