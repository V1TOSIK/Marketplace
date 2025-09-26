namespace AuthModule.Application.Dtos.Requests
{
    public class SendVerificationCodeRequest
    {
        public string Destination { get; set; } = string.Empty;
    }
}
