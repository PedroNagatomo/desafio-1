using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hypesoft.Domain.Exceptions
{
    public class InsufficientStockException : DomainException
    {
        public InsufficientStockException(string productName, int available, int requested)
            : base($"Insufficient stock for product '{productName}'. Available: {available}, Requested: {requested}")
        {
        }
    }
}