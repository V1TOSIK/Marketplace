namespace AuthUserModule.Domain.Exceptions
{
    public class InvalidUserRoleException : Exception
    {
        public InvalidUserRoleException() : base("Invalid user role specified.")
        {
        }

        public InvalidUserRoleException(string message) : base(message)
        {
        }
        
        public InvalidUserRoleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
