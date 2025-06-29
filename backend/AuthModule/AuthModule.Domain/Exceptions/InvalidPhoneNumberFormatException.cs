using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class InvalidPhoneNumberFormatException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public InvalidPhoneNumberFormatException(string message)
            : base(message) { }

        public InvalidPhoneNumberFormatException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidPhoneNumberFormatException()
            : base("PhoneNumber is not in a valid format.") { }

        public InvalidPhoneNumberFormatException(Exception innerException)
            : base("PhoneNumber is not in a valid format.", innerException) { }
    }
}
