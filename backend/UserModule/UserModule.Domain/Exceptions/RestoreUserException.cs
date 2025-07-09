using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class RestoreUserException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

        public RestoreUserException(string message)
            : base(message) { }
        public RestoreUserException(string message, Exception innerException)
            : base(message, innerException) { }
        public RestoreUserException()
            : base("User cannot be restored because it is not deleted.") { }
        public RestoreUserException(Exception innerException)
            : base("User cannot be restored because it is not deleted.", innerException) { }


    }
}
