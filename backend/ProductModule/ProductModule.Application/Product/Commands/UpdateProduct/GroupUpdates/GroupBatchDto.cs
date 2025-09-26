namespace ProductModule.Application.Product.Commands.UpdateProduct.GroupUpdates
{
    public class GroupBatchDto
    {
        public List<GroupCreateDto>? Added { get; set; }
        public List<GroupUpdateDto>? Updated { get; set; }
        public List<int>? Deleted { get; set; }
    }
}
