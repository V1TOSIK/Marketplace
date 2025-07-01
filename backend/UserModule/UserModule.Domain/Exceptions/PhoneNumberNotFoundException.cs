using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class PhoneNumberNotFoundException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
        public PhoneNumberNotFoundException(string message)
            : base(message) { }
        public PhoneNumberNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
        public PhoneNumberNotFoundException()
            : base("Phone number not found.") { }
        public PhoneNumberNotFoundException(Exception innerException)
            : base("Phone number not found.", innerException) { }
    }
}
