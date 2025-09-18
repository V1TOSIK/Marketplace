using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductModule.Domain.Enums;
using ProductModule.Domain.Exceptions;
using ProductModule.Domain.ValueObjects;
using SharedKernel.AgregateRoot;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ProductModule.Domain.Entities
{
    public class Product : AggregateRoot<Guid>
    {
        private Product() { }
        private Product(Guid userId,
            string name,
            Price price,
            string location,
            string description,
            int categoryId,
            Status status)
        {
            UserId = userId;
            Name = name;
            Price = price;
            Location = location;
            Description = description;
            CategoryId = categoryId;
            Status = status;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid UserId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Price Price { get; private set; }
        public string Location { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Status Status { get; private set; } = Status.Draft;
        public DateTime CreatedAt { get; private set; }

        public int CategoryId { get; private set; }

        private readonly List<CharacteristicGroup> _characteristicGroups = new();
        public IReadOnlyCollection<CharacteristicGroup> CharacteristicGroups => _characteristicGroups.AsReadOnly();

        public static Product Create(Guid userId,
            string name,
            decimal priceAmount,
            string priceCurrency,
            string location,
            string description,
            int categoryId,
            string statusValue)
        {
            var price = new Price(priceAmount, priceCurrency);

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidProductDataException("Product name cannot be empty or null.");
            if (string.IsNullOrWhiteSpace(location))
                throw new InvalidProductDataException("Location cannot be empty or null.");
            if (string.IsNullOrWhiteSpace(description))
                throw new InvalidProductDataException("Description cannot be empty or null.");
            if (categoryId <= 0)
                throw new InvalidProductDataException("Category ID must be a positive integer.");
            if (!Enum.TryParse<Status>(statusValue, true, out var status))
                throw new InvalidProductDataException("Invalid product status.");

            return new Product(userId, name, price, location, description, categoryId, status);
        }

        public void UpdateProduct(string? name, decimal? priceAmount, string? priceCurrency, string? location, string? description, int? categoryId)
        {
            if (!string.IsNullOrWhiteSpace(name))
                UpdateName(name);
            if (!string.IsNullOrWhiteSpace(priceCurrency) && priceAmount.HasValue)
                UpdatePrice(priceAmount.Value, priceCurrency);
            if (!string.IsNullOrWhiteSpace(location))
                UpdateLocation(location);
            if (!string.IsNullOrWhiteSpace(description))
                UpdateDescription(description);
            if (categoryId.HasValue)
                UpdateCategory(categoryId.Value);
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidProductDataException("Product name cannot be empty or null.");
            Name = name;
        }

        public void UpdatePrice(decimal priceAmount, string priceCurrency)
        {
            Price = new Price(priceAmount, priceCurrency);
        }

        public void UpdateLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new InvalidProductDataException("Location cannot be empty or null.");
            Location = location;
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new InvalidProductDataException("Description cannot be empty or null.");
            Description = description;
        }

        public void UpdateCategory(int categoryId)
        {
            if (categoryId <= 0)
                throw new InvalidProductDataException("Category ID must be a positive integer.");
            CategoryId = categoryId;
        }

        public void Publish()
        {
            if (Status == Status.Published)
                throw new InvalidProductOperationException("Product is already published.");
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Location) ||
                string.IsNullOrWhiteSpace(Description) ||
                Price.Amount <= 0)
            {
                throw new InvalidProductOperationException("Cannot publish product with incomplete data.");
            }
            Status = Status.Published;
        }

        public void UpdateStatus(Status status)
        {
            if (status == Status.Published)
            {
                if (string.IsNullOrWhiteSpace(Name) ||
                    string.IsNullOrWhiteSpace(Location) ||
                    string.IsNullOrWhiteSpace(Description) ||
                    Price.Amount <= 0)
                {
                    throw new InvalidProductOperationException("Cannot publish product with incomplete data.");
                }
            }

            Status = status;
        }

        public void AddCharacteristicGroup(CharacteristicGroup characteristicGroup)
        {
            if (characteristicGroup == null)
                throw new InvalidCharacteristicGroupDataException("Characteristic group cannot be null.");

            _characteristicGroups.Add(characteristicGroup);
        }

        public void RemoveCharacteristicGroup(int groupId)
        {
            var group = _characteristicGroups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
                throw new InvalidCharacteristicGroupDataException("Characteristic group not found.");
            _characteristicGroups.Remove(group);
        }

        public void UpdateCharacteristicGroup(int groupId, string? name)
        {
            var group = _characteristicGroups.FirstOrDefault(x => x.Id == groupId);
            group?.UpdateName(name);
        }

        public void RemoveCharacteristic(int charId)
        {
            var characteristic = _characteristicGroups
            .SelectMany(g => g.CharacteristicValues)
                .FirstOrDefault(c => c.Id == charId);

            if (characteristic == null)
                throw new InvalidCharacteristicValueDataException("Characteristic not found.");

            var group = _characteristicGroups.FirstOrDefault(g => g.Id == characteristic.GroupId);
            group?.RemoveCharacteristic(characteristic.Id);
        }

        public async Task AddCharacteristic(int groupId, string value, Func<Task<int>> getTemplateId)
        {
            var group = _characteristicGroups.FirstOrDefault(g => g.Id == groupId)
                ?? throw new InvalidCharacteristicGroupDataException("Characteristic group not found.");

            var templateId = await getTemplateId();
            group.AddCharacteristic(value, templateId);
        }

        public void UpdateCharacteristic(int charId, string? value)
        {
            var characteristic = _characteristicGroups
                .SelectMany(g => g.CharacteristicValues)
                .FirstOrDefault(c => c.Id == charId);

            characteristic?.UpdateValue(value);
        }
    }
}
