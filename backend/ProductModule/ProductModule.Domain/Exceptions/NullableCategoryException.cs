using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class NullableCategoryException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public NullableCategoryException(string message)
            : base(message) { }

        public NullableCategoryException(string message, Exception innerException)
            : base(message, innerException) { }

        public NullableCategoryException()
            : base("Category cannot be null.") { }

        public NullableCategoryException(Exception innerException)
            : base("Category cannot be null.", innerException) { }
    }
}
