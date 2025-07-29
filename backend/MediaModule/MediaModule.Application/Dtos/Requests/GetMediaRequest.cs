namespace MediaModule.Application.Dtos.Requests
{
    public class GetMediaRequest
    {
        public Guid EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
    }
}
