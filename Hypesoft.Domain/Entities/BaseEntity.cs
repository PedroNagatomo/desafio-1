using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hypesoft.Domain.Entities;

public abstract class BaseEntity{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id{get; protected set;} = ObjectId.GenerateNewId().ToString();

    public DateTime createdAt{get; protected set;} = DateTime.UtcNow;
    public DateTime UpdatedAt{get; protected set;} = DateTime.UtcNow;

    public void Updatetimestamp(){
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTimestamp()
    {
        throw new NotImplementedException();
    }
}