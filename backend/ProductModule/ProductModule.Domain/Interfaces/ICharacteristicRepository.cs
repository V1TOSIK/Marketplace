using ProductModule.Domain.Entities;

namespace ProductModule.Domain.Interfaces
{
    public interface ICharacteristicRepository
    {
        Task<IEnumerable<CharacteristicGroup>> GetAllAsync(Guid productId);
        Task<CharacteristicGroup> GetByIdAsync(int groupId);
        Task DeleteAsync(int groupId);

        Task<IEnumerable<CharacteristicValue>> GetValuesByGroupIdAsync(int groupId);
        Task DeleteValueAsync(string value, int templateId, int groupId);

        Task AddTemplateAsync(CharacteristicTemplate template);
        Task<CharacteristicTemplate?> GetTemplateByPropertiesAsync(string templateName, string unit);
        Task<CharacteristicTemplate> GetOrCreateTemplateAsync(string templateName, int categoryId, string unit);

    }
}
