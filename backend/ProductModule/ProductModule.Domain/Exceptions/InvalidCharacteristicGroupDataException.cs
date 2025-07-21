using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class InvalidCharacteristicGroupDataException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public InvalidCharacteristicGroupDataException(string message)
            : base(message) { }

        public InvalidCharacteristicGroupDataException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidCharacteristicGroupDataException()
            : base("Invalid characteristic group data.") { }

        public InvalidCharacteristicGroupDataException(Exception innerException)
            : base("Invalid characteristic group data.", innerException) { }
    }
}