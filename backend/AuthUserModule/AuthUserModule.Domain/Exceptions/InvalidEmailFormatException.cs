namespace AuthUserModule.Domain.Exceptions
{
    public class InvalidEmailFormatException : Exception
    {
        public InvalidEmailFormatException() : base("Invalid email format")
        {
        }

        public InvalidEmailFormatException(string? message) : base(message)
        {
        }

        public InvalidEmailFormatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
