namespace ProductModule.Application.Dtos
{
    public class GroupBatchDto
    {
        public List<GroupCreateDto> Added { get; set; } = new();
        public List<GroupUpdateDto> Updated { get; set; } = new();
        public List<int> Deleted { get; set; } = new();
    }
}
