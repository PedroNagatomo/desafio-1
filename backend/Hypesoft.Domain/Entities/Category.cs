using MongoDB.Bson.Serialization.Attributes;

namespace Hypesoft.Domain.Entities;

public class Category : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; private set; }

    [BsonElement("description")]
    public string? Description { get; private set; }

    [BsonElement("isActive")]
    public bool IsActive { get; private set; }

    protected Category() { }

    public Category(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        IsActive = true;
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

    public void UpdateInfo(string name, string? description)
    {
        throw new NotImplementedException();
    }
}