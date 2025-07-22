using Microsoft.EntityFrameworkCore;
using ProductModule.Domain.Entities;

namespace ProductModule.Domain.Interfaces
{
    public interface ICharacteristicRepository
    {
        Task<IEnumerable<CharacteristicGroup>> GetProductGroupsAsync(Guid productId);
        Task<CharacteristicGroup> GetGroupByIdAsync(int groupId);
        Task DeleteGroupAsync(int groupId);

        Task<IEnumerable<CharacteristicValue>> GetValuesByGroupIdAsync(int groupId);
        Task DeleteValueAsync(string value, int templateId, int groupId);

        Task AddTemplateAsync(CharacteristicTemplate template);
        Task<CharacteristicTemplate?> GetTemplateByPropertiesAsync(string templateName, string unit);
        Task<List<CharacteristicTemplate>> GetTemplatesByIdsAsync(List<int> templateIds);
        Task<CharacteristicTemplate> GetOrCreateTemplateAsync(string templateName, int categoryId, string unit);

    }
}
