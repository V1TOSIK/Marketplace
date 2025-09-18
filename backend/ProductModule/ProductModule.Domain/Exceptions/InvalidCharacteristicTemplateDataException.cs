using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class InvalidCharacteristicTemplateDataException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public InvalidCharacteristicTemplateDataException(string message)
            : base(message) { }

        public InvalidCharacteristicTemplateDataException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidCharacteristicTemplateDataException()
            : base("Invalid characteristic template data.") { }

        public InvalidCharacteristicTemplateDataException(Exception innerException)
            : base("Invalid characteristic template data.", innerException) { }
    }
}