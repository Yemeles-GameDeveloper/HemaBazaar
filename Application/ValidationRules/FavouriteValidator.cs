using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ValidationRules
{
    internal class FavouriteValidator : AbstractValidator<Favourite>
    {
        public FavouriteValidator()
        {
          
            RuleFor(x => x.AppUserId)
                .GreaterThan(0).WithMessage("KullanıcıId sıfırdan büyük olmalıdur.");
            RuleFor(x => x.ItemId)
                .GreaterThan(0).WithMessage("EşyaId sıfırdan büyük olmalıdur.");
        }
    }
}
