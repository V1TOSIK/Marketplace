namespace ProductModule.Application.Product.Commands.UpdateProduct.CharacteristicUpdates
{
    public class CharacteristicUpdateDto
    {
        public int Id { get; set; }
        public string? Value { get; set; }
        public string? Unit { get; set; }
    }
}
