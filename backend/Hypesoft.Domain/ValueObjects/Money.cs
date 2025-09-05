using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Hypesoft.Domain.ValueObjects;

[BsonIgnoreExtraElements]
public class Money : IEquatable<Money>
{
    [BsonElement("amount")]
    public decimal Amount { get; private set; }

    [BsonElement("currency")]
    public string Currency { get; private set; }

    protected Money()
    {
        Currency = "BRL";
    }

    public Money(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        Amount = Math.Round(amount, 2);
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        ValidateSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        ValidateSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal mutiplier)
    {
        return new Money(Amount * mutiplier, Currency);
    }

    public Money Divide(decimal divisor)
    {
        if (divisor == 0)
            throw new ArgumentException("Cannot divide by zero", nameof(divisor));

        return new Money(Amount / divisor, Currency);
    }

    public bool IsGreaterThan(Money other)
    {
        ValidateSameCurrency(other);
        return Amount > other.Amount;
    }

    public bool IsLessThan(Money other)
    {
        ValidateSameCurrency(other);
        return Amount < other.Amount;
    }

    private void ValidateSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot operate on different currencies: {Currency} and {other.Currency}");
    }

    public override string ToString()
    {
        return Currency switch
        {
            "BRL" => $"R$ {Amount:N2}",
            "USD" => $"$ {Amount:N2}",
            "EUR" => $"€ {Amount:N2}",
            _ => $"{Amount:N2} {Currency}"
        };
    }

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj)
    {
        return obj is Money other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public static bool operator ==(Money? left, Money? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !Equals(left, right);
    }

    public static Money operator +(Money left, Money right)
    {
        return left.Add(right);
    }

    public static Money operator -(Money left, Money right)
    {
        return left.Subtract(right);
    }

    public static Money operator *(Money left, decimal right)
    {
        return left.Multiply(right);
    }

    public static Money operator /(Money left, decimal right)
    {
        return left.Divide(right);
    }

    public static implicit operator Money(decimal amount)
    {
        return new Money(amount);
    }
}
