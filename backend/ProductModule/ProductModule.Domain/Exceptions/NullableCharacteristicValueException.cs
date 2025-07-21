using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class NullableCharacteristicValueException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public NullableCharacteristicValueException(string message)
            : base(message) { }

        public NullableCharacteristicValueException(string message, Exception innerException)
            : base(message, innerException) { }

        public NullableCharacteristicValueException()
            : base("Characteristic value cannot be null.") { }

        public NullableCharacteristicValueException(Exception innerException)
            : base("Characteristic value cannot be null.", innerException) { }
    }
}
