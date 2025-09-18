using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class PhoneNumberIsAlreadyAddedException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public PhoneNumberIsAlreadyAddedException(string message)
            : base(message) { }
        public PhoneNumberIsAlreadyAddedException(string message, Exception innerException)
            : base(message, innerException) { }
        public PhoneNumberIsAlreadyAddedException()
            : base("Phone number is already added to the user.") { }
        public PhoneNumberIsAlreadyAddedException(Exception innerException)
            : base("Phone number is already added to the user.", innerException) { }
    }
}
