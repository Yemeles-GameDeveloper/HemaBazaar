using Application.DTOs;
using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ValidationRules
{
    internal class CategoryDTOValidator : AbstractValidator<CategoryDTO>
    {
        public CategoryDTOValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Başlık boş olamaz.")
                .MaximumLength(40).WithMessage("Kategori ismi 40 karakterden büyük olamaz");
                

               
        }
    }
}
