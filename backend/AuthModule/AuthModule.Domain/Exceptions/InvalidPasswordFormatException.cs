namespace AuthModule.Domain.Exceptions
{
    internal class InvalidPasswordFormatException : Exception
    {
        public InvalidPasswordFormatException() : base("Invalid password format")
        {
        }

        public InvalidPasswordFormatException(string? message) : base(message)
        {
        }

        public InvalidPasswordFormatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
