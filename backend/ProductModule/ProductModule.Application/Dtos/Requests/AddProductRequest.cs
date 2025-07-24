namespace ProductModule.Application.Dtos.Requests
{
    public class AddProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal PriceAmount { get; set; }
        public string PriceCurrency { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }

        public List<CharacteristicGroupRequest> CharacteristicGroups { get; set; } = new ();
    }
}
