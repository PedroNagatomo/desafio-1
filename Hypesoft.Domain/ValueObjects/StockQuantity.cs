using MongoDB.Bson.Serialization.Attributes;

namespace Hypesoft.Domain.ValueObjects;

[BsonIgnoreExtraElements]
public class StockQuantity : IEquatable<StockQuantity>
{
    [BsonElement("value")]
    public int Value { get; private set; }
    protected StockQuantity() { }
    public StockQuantity(int value)
    {
        if (value < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(value));

        Value = value;
    }

    public bool IsEmpty => Value == 0;
    public bool IsLow(int threshold = 10) => Value < threshold;
    public bool IsAvailable => Value > 0;

    public StockQuantity Add(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity to add cannot be negative", nameof(quantity));

        return new StockQuantity(Value + quantity);
    }

    public StockQuantity Remove(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity to remove cannot be negative", nameof(quantity));

        if (Value < quantity)
            throw new InvalidOperationException("Insufficient stock");

        return new StockQuantity(Value - quantity);
    }

    public override string ToString()
    {
        return Value switch
        {
            0 => "Out of stock",
            1 => "1 unit",
            _ => $"{Value} units"
        };
    }

    public bool Equals(StockQuantity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is StockQuantity other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    public static bool operator ==(StockQuantity? left, StockQuantity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(StockQuantity? left, StockQuantity? right)
    {
        return !Equals(left, right);
    }

    public static bool operator >(StockQuantity left, StockQuantity right)
    {
        return left.Value > right.Value;
    }

    public static bool operator <(StockQuantity left, StockQuantity right)
    {
        return left.Value < right.Value;
    }

    public static bool operator >=(StockQuantity left, StockQuantity right)
    {
        return left.Value >= right.Value;
    }

    public static bool operator <=(StockQuantity left, StockQuantity right)
    {
        return left.Value <= right.Value;
    }

    public static implicit operator StockQuantity(int value)
    {
        return new StockQuantity(value);
    }

    public static implicit operator int(StockQuantity stock)
    {
        return stock.Value;
    }
    
}