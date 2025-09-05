using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Hypesoft.Domain.ValueObjects;
using System.Data.SqlTypes;

namespace Hypesoft.Domain.Entities
{
    public class Product : BaseEntity
    {
        [BsonElement("name")]
        public string Name { get; private set; }

        [BsonElement("description")]
        public string? Description { get; private set; }

        [BsonElement("price")]
        public Money Price { get; private set; }

        [BsonElement("categoryId")]
        public string CategoryId { get; private set; }

        [BsonElement("stock")]
        public StockQuantity Stock { get; private set; }

        [BsonElement("isActive")]
        public bool IsActive { get; private set; }

        [BsonElement("sku")]
        public string? SKU { get; private set; }

        protected Product() { }

        public Product(String name, string? description, Money price, string categoryId, StockQuantity stock, string? sku = null)
        {
            ValidateName(name);
            ValidateCategoryId(categoryId);

            Name = name.Trim();
            Description = description?.Trim();
            Price = price ?? throw new ArgumentNullException(nameof(price));
            CategoryId = categoryId;
            SKU = sku?.Trim();
            Updatetimestamp();
        }

        public void UpdateStock(int quantity)
        {
            Stock = new StockQuantity(quantity);
            Updatetimestamp();
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity to remove must be positive", nameof(quantity));

            if (Stock.Value < quantity)
                throw new InvalidOperationException("Insufficient stock");

            Stock = new StockQuantity(Stock.Value - quantity);
            Updatetimestamp();
        }

        public bool IsLowStock(int threshold = 10)
        {
            return Stock.Value < threshold;
        }

        public void Activate()
        {
            IsActive = true;
            Updatetimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            Updatetimestamp();
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty", nameof(name));

            if (name.Length > 200)
                throw new ArgumentException("Product name cannot exceed 200 characters", nameof(name));
        }

        private static void ValidateCategoryId(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                throw new ArgumentException("CategoryId cannot be empty", nameof(categoryId));
        }

        public void AddStock(int quantity)
        {
            throw new NotImplementedException();
        }

        public void UpdateInfo(string name, string? description, Money money, string categoryId, string? sKU)
        {
            throw new NotImplementedException();
        }
    }
}