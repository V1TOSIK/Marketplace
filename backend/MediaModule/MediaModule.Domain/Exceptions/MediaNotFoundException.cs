using SharedKernel.Exceptions;
using System.Net;

namespace MediaModule.Domain.Exceptions
{
    public class MediaNotFoundException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
        public MediaNotFoundException(string message)
            : base(message) { }
        public MediaNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
        public MediaNotFoundException()
            : base("Media not found.") { }
        public MediaNotFoundException(Exception innerException)
            : base("Media not found.", innerException) { }
    }
}
