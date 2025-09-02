using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class InvalidProviderException : BaseException
    {
        public override HttpStatusCode StatusCode => base.StatusCode;

        public InvalidProviderException(string message)
            : base(message) { }

        public InvalidProviderException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidProviderException()
            : base("Invalid provider data.") { }

        public InvalidProviderException(Exception innerException)
            : base("Invalid provider data.", innerException) { }
    }
}
