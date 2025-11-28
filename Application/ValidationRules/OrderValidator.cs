using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ValidationRules
{
    internal class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(x => x.AppUserId)
                .NotEmpty().WithMessage("KullanıcıId boş geçilemez.");
            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("Sipariş tarihi boş geçilemez.");
            RuleFor(x => x.Address)
                .NotEmpty().MinimumLength(10).WithMessage("Adres 10 karakterden küçük olamaz");
        }
    }
}
