using SharedKernel.Exceptions;
using System.Net;

namespace ProductModule.Domain.Exceptions
{
    public class InvalidCategoryDataException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public InvalidCategoryDataException(string message)
            : base(message) { }

        public InvalidCategoryDataException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidCategoryDataException()
            : base("Invalid category data.") { }

        public InvalidCategoryDataException(Exception innerException)
            : base("Invalid category data.", innerException) { }
    }
}
