using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductModule.Domain.Entities;
using ProductModule.Domain.Exceptions;
using ProductModule.Domain.Interfaces;

namespace ProductModule.Persistence.Repositories
{
    public class CharacteristicRepository : ICharacteristicRepository
    {
        private readonly ProductDbContext _dbContext;
        private readonly ILogger<CharacteristicRepository> _logger;
        public CharacteristicRepository(ProductDbContext dbContext,
            ILogger<CharacteristicRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<CharacteristicGroup>> GetProductGroupsAsync(Guid productId)
        {
            return await _dbContext.CharacteristicGroups
                .Where(c => c.ProductId == productId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CharacteristicGroup> GetGroupByIdAsync(int groupId)
        {
            var group = await _dbContext.CharacteristicGroups
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
                throw new NullableCharacteristicGroupException("group cannot be null");

            return group;
        }

        public async Task<IEnumerable<CharacteristicValue>> GetValuesByGroupIdAsync(int groupId)
        {
            return await _dbContext.CharacteristicValues
                .Where(cv => cv.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CharacteristicTemplate>> GetTemplatesByIdsAsync(List<int> templateIds)
        {
            return await _dbContext.CharacteristicTemplates
                .Where(t => templateIds.Contains(t.Id))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddTemplateAsync(CharacteristicTemplate template)
        {
            if (template == null)
                throw new NullableCharacteristicTemplateException("Template cannot be null");

            await _dbContext.CharacteristicTemplates.AddAsync(template);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CharacteristicTemplate?> GetTemplateByPropertiesAsync(string templateName, string unit)
        {
            var lowerTemplateName = templateName.ToLowerInvariant();
            var lowerUnit = unit.ToLowerInvariant();

            var template = await _dbContext.CharacteristicTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t =>
                t.Name.ToLower() == lowerTemplateName &&
                t.Unit.ToLower() == lowerUnit);

            return template;
        }

        public async Task<CharacteristicTemplate> GetOrCreateTemplateAsync(string templateName, int categoryId, string unit)
        {
            var template = await GetTemplateByPropertiesAsync(templateName, unit);
            if (template == null)
            {
                template = CharacteristicTemplate.Create(templateName, unit, categoryId, "Custom");
                await AddTemplateAsync(template);
            }
            return template;
        }

        public async Task DeleteGroupAsync(int groupId)
        {
            await _dbContext.CharacteristicValues
                .Where(cv => cv.GroupId == groupId)
                .AsNoTracking()
                .ExecuteDeleteAsync();

            await _dbContext.CharacteristicGroups
                .Where(g => g.Id == groupId)
                .AsNoTracking()
                .ExecuteDeleteAsync();

            _logger.LogInformation($"Characteristic group with ID {groupId} deleted successfully.");
        }

        public async Task DeleteValueAsync(string value, int templateId, int groupId)
        {
            var characteristicValue = await _dbContext.CharacteristicValues
                .FirstOrDefaultAsync(cv => cv.Value == value && cv.CharacteristicTemplateId == templateId && cv.GroupId == groupId);
            if (characteristicValue == null)
            {
                _logger.LogWarning($"Characteristic value with value '{value}', template ID {templateId}, and group ID {groupId} not found.");
                throw new NullableCharacteristicValueException("Characteristic value not found.");
            }
            _dbContext.CharacteristicValues.Remove(characteristicValue);
            _logger.LogInformation($"Characteristic value '{value}' deleted successfully.");
        }
    }
}
