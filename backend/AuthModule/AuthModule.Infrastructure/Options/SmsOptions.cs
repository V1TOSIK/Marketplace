namespace AuthModule.Infrastructure.Options
{
    public class SmsOptions
    {
        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string FromPhone { get; set; } = string.Empty;
    }
}
