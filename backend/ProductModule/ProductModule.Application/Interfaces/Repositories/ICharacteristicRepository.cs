using ProductModule.Application.Dtos;
using ProductModule.Domain.Entities;

namespace ProductModule.Application.Interfaces.Repositories
{
    public interface ICharacteristicRepository
    {
        Task DeleteGroupAsync(int groupId, CancellationToken cancellationToken);
        Task DeleteValueAsync(string value, int templateId, int groupId, CancellationToken cancellationToken);

        Task<IEnumerable<CharacteristicGroupDto>> GetProductCharacterisitcsAsync(Guid productId, CancellationToken cancellationToken);

        Task AddTemplateAsync(CharacteristicTemplate template, CancellationToken cancellationToken);
        Task<CharacteristicTemplate?> GetTemplateByPropertiesAsync(string templateName, string unit, CancellationToken cancellationToken);
        Task<CharacteristicTemplate> GetOrCreateTemplateAsync(string templateName, int categoryId, string unit, CancellationToken cancellationToken);

    }
}
