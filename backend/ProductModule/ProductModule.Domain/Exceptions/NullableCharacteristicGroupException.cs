using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class NullableCharacteristicGroupException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public NullableCharacteristicGroupException(string message)
            : base(message) { }

        public NullableCharacteristicGroupException(string message, Exception innerException)
            : base(message, innerException) { }

        public NullableCharacteristicGroupException()
            : base("Characteristic group cannot be null.") { }

        public NullableCharacteristicGroupException(Exception innerException)
            : base("Characterisitc group cannot be null.", innerException) { }
    }
}
