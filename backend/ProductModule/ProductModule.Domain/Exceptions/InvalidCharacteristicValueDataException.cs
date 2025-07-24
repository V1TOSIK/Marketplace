using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class InvalidCharacteristicValueDataException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public InvalidCharacteristicValueDataException(string message)
            : base(message) { }

        public InvalidCharacteristicValueDataException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidCharacteristicValueDataException()
            : base("Invalid characteristic value data.") { }

        public InvalidCharacteristicValueDataException(Exception innerException)
            : base("Invalid characteristic value data.", innerException) { }
    }
}