using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hypesoft.Application.DTOs;
using MediatR;

namespace Hypesoft.Application.Commands.Categories
{
    public class UpdateCategoryCommand : IRequest<CategoryDto>
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public UpdateCategoryCommand() { }

        public UpdateCategoryCommand(string id, UpdateCategoryDto dto)
        {
            Id = id;
            Name = dto.Name;
            Description = dto.Description;
        }
    }
}