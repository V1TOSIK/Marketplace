using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class PhoneNumberAlreadyExistsException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

        public PhoneNumberAlreadyExistsException(string message)
            : base(message) { }

        public PhoneNumberAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException) { }

        public PhoneNumberAlreadyExistsException()
            : base("User with phone number already exists.") { }

        public PhoneNumberAlreadyExistsException(Exception innerException)
            : base("User with phone number already exists.", innerException) { }
    }
}
