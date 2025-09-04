using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hypesoft.Domain.Exceptions
{
    public class CategoryInUseException : DomainException
    {
        public CategoryInUseException(string categoryName)
            : base($"Category '{categoryName}' cannot be deleted because it has associated products."){}
    }
}