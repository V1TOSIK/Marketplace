namespace AuthModule.Domain.Exceptions
{
    public class RefreshTokenException : Exception
    {
        public RefreshTokenException() : base("An error occurred with the refresh token operation.")
        {
        }

        public RefreshTokenException(string message) : base(message)
        {
        }

        public RefreshTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
