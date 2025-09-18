using SharedKernel.Exceptions;
using System.Net;

namespace AuthModule.Domain.Exceptions
{
    public class InvalidUserRoleException : BaseException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public InvalidUserRoleException(string message)
            : base(message) { }

        public InvalidUserRoleException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidUserRoleException()
            : base("User role is not in a valid format.") { }

        public InvalidUserRoleException(Exception innerException)
            : base("User role is not in a valid format.", innerException) { }
    }
}
