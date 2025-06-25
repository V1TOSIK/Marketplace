namespace AuthModule.Domain.Exceptions
{
    public class UserBlockExсeption : Exception
    {
        public UserBlockExсeption() : base("User is not blocked")
        {
        }
        public UserBlockExсeption(string? message) : base(message)
        {
        }
        public UserBlockExсeption(string? message, Exception? innerException) : base(message, innerException)
        {
        }
        public UserBlockExсeption(Exception? innerException) : base("User is not blocked", innerException)
        {
        }
    }
}
