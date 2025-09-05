using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Hypesoft.Application.Commands.Categories
{
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;

        public DeleteCategoryCommand() { }

        public DeleteCategoryCommand(string id)
        {
            Id = id;
        }
    }
}