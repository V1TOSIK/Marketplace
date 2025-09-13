using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductModule.Domain.Entities;
using ProductModule.Domain.Exceptions;
using ProductModule.Application.Interfaces.Repositories;
using ProductModule.Application.Dtos;

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

        public async Task<IEnumerable<CharacteristicGroupDto>> GetProductCharacterisitcsAsync(Guid productId, CancellationToken cancellationToken)
        {
            var result = await (
                from g in _dbContext.CharacteristicGroups
                join v in _dbContext.CharacteristicValues on g.Id equals v.GroupId
                join t in _dbContext.CharacteristicTemplates on v.CharacteristicTemplateId equals t.Id
                where g.ProductId == productId
                select new
                {
                    GroupId = g.Id,
                    GroupName = g.Name,
                    CharacteristicId = v.Id,
                    CharacteristicName = t.Name,
                    CharacteristicValue = v.Value,
                    CharacteristicUnit = t.Unit
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var response = result
                .GroupBy(x => new { x.GroupId, x.GroupName })
                .Select(g => new CharacteristicGroupDto
                {
                    Id = g.Key.GroupId,
                    Name = g.Key.GroupName,
                    Characteristics = g.Select(c => new CharacteristicDto
                    {
                        Id = c.CharacteristicId,
                        Name = c.CharacteristicName,
                        Value = c.CharacteristicValue,
                        Unit = c.CharacteristicUnit
                    }).ToList()
                })
                .ToList();

            return response;
        }

        public async Task AddTemplateAsync(CharacteristicTemplate template, CancellationToken cancellationToken)
        {
            if (template == null)
            {
                _logger.LogError("[Product Module(Repository)] Attempted to add a null CharacteristicTemplate.");
                throw new NullableCharacteristicTemplateException("Template cannot be null");
            }

            await _dbContext.CharacteristicTemplates.AddAsync(template, cancellationToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CharacteristicTemplate?> GetTemplateByPropertiesAsync(string templateName, string unit, CancellationToken cancellationToken)
        {
            var lowerTemplateName = templateName.ToLowerInvariant();
            var lowerUnit = unit.ToLowerInvariant();

            var template = await _dbContext.CharacteristicTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t =>
                t.Name.ToLower() == lowerTemplateName &&
                t.Unit.ToLower() == lowerUnit, cancellationToken);

            return template;
        }

        public async Task<CharacteristicTemplate> GetOrCreateTemplateAsync(string templateName, int categoryId, string unit, CancellationToken cancellationToken)
        {
            var template = await GetTemplateByPropertiesAsync(templateName, unit, cancellationToken);
            if (template == null)
            {
                template = CharacteristicTemplate.Create(templateName, unit, categoryId, "Custom");
                await AddTemplateAsync(template, cancellationToken);
            }
            return template;
        }
    }
}
