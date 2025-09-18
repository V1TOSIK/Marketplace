namespace AuthModule.Application.Dtos.Requests
{
    public class VerificationRequest
    {
        public string Destination { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
