using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class UnbanUserException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public UnbanUserException(string message)
            : base(message) { }
        public UnbanUserException(string message, Exception innerException)
            : base(message, innerException) { }
        public UnbanUserException()
            : base("An error occurred while unban the user.") { }
        public UnbanUserException(Exception innerException)
            : base("An error occurred while unban the user.", innerException) { }
    }
}