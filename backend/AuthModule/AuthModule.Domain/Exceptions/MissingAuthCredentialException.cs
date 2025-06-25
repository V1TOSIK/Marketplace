namespace AuthModule.Domain.Exceptions
{
    internal class MissingAuthCredentialException : Exception
    {
        public MissingAuthCredentialException()
            : base("At least one of email or phone number must be provided.") { }

        public MissingAuthCredentialException(string? message) : base(message)
        {
        }

        public MissingAuthCredentialException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
