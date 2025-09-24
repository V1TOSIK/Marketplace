using System.Net;

namespace SharedKernel.Exceptions
{
    public class MyValidationException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public MyValidationException(string message)
            : base(message) { }

        public MyValidationException(string message, Exception innerException)
            : base(message, innerException) { }

        public MyValidationException()
            : base("PhoneNumber is not in a valid format.") { }

        public MyValidationException(Exception innerException)
            : base("PhoneNumber is not in a valid format.", innerException) { }
    }
}
