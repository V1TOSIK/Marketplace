using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class NullablePhoneNumberException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public NullablePhoneNumberException(string message)
            : base(message) { }
        public NullablePhoneNumberException(string message, Exception innerException)
            : base(message, innerException) { }
        public NullablePhoneNumberException()
            : base("Phone number cannot be null or empty.") { }
        public NullablePhoneNumberException(Exception innerException)
            : base("Phone number cannot be null or empty.", innerException) { }
    }
}
