using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hypesoft.Domain.Exceptions
{
    public class DuplicateProductException : DomainException
    {
        public DuplicateProductException(string productName)
            : base($"A product with name '{productName}' already exists."){}
    }
}