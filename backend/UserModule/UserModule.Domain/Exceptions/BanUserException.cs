using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class BanUserException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public BanUserException(string message)
            : base(message) { }
        public BanUserException(string message, Exception innerException)
            : base(message, innerException) { }
        public BanUserException()
            : base("An error occurred while ban the user.") { }
        public BanUserException(Exception innerException)
            : base("An error occurred while ban the user.", innerException) { }
    }
}
