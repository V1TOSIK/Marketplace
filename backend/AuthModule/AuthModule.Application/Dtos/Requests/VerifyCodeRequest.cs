namespace AuthModule.Application.Dtos.Requests
{
    public class VerifyCodeRequest
    {
        public string Destination { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
