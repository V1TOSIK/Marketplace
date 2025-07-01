using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class BlockNotFoundException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
        public BlockNotFoundException(string message)
            : base(message) { }
        public BlockNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
        public BlockNotFoundException()
            : base("Block not found.") { }
        public BlockNotFoundException(Exception innerException)
            : base("Block not found.", innerException) { }
    }
}
