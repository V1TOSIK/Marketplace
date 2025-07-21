using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class NullableCharacteristicTemplateException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public NullableCharacteristicTemplateException(string message)
            : base(message) { }

        public NullableCharacteristicTemplateException(string message, Exception innerException)
            : base(message, innerException) { }

        public NullableCharacteristicTemplateException()
            : base("Characteristic template cannot be null.") { }

        public NullableCharacteristicTemplateException(Exception innerException)
            : base("Characterisitc template cannot be null.", innerException) { }
    }
}
