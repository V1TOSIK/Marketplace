using SharedKernel.Exceptions;
using System.Net;

namespace UserModule.Domain.Exceptions
{
    public class BlockExistException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public BlockExistException(string message)
            : base(message) { }
        public BlockExistException(string message, Exception innerException)
            : base(message, innerException) { }
        public BlockExistException()
            : base("Block already exists.") { }
        public BlockExistException(Exception innerException)
            : base("Block already exists.", innerException) { }
    }
}
