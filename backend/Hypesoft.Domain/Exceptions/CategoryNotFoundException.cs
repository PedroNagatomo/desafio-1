using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hypesoft.Domain.Exceptions
{
    public class CategoryNotFoundException : DomainException
    {
        public CategoryNotFoundException(string categoryId)
            : base($"Category with id '{categoryId}' was not found."){}
    }
}