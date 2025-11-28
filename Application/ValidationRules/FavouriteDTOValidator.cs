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
    internal class FavouriteDTOValidator : AbstractValidator<FavouriteDTO>
    {
        public FavouriteDTOValidator()
        {
          
            RuleFor(x => x.UserName)
                .NotEmpty().MinimumLength(1).WithMessage("Kullanıcı adı sıfırdan büyük olmalıdır.");
            RuleFor(x => x.ItemTitle)
                .NotEmpty().MinimumLength(1).WithMessage("Eşya adı sıfırdan büyük olmalıdır.");
        }
    }
}
