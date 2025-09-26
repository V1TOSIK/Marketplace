namespace ProductModule.Application.Product.Commands.UpdateProduct.CharacteristicUpdates
{
    public class CharacteristicBatchDto
    {
        public List<CharacteristicCreateDto>? Added { get; set; }
        public List<CharacteristicUpdateDto>? Updated { get; set; }
        public List<int>? Deleted { get; set; }
    }
}
