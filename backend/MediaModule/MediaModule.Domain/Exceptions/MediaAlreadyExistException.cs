using SharedKernel.Exceptions;
using System.Net;

namespace MediaModule.Domain.Exceptions
{
    public class MediaAlreadyExistException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

        public MediaAlreadyExistException(string message)
            : base(message) { }
        public MediaAlreadyExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public MediaAlreadyExistException()
            : base("Media already exist.") { }
        public MediaAlreadyExistException(Exception innerException)
            : base("Media already exist.", innerException) { }
    }
}
