namespace AuthUserModule.Domain.Exceptions
{
    internal class InvalidPhoneNumberFormatException : Exception
    {
        public InvalidPhoneNumberFormatException() : base("Invalid phone number format")
        {
        }

        public InvalidPhoneNumberFormatException(string? message) : base(message)
        {
        }

        public InvalidPhoneNumberFormatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
