namespace MediaModule.Application.Dtos.Responses
{
    public class MediaResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public string Url { get; set; } = null!;
    }
}
