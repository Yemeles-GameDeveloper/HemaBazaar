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
    internal class CartDTOValidator : AbstractValidator<CartDTO>
    {
        public CartDTOValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Miktar sıfırdan büyük olmalıdır.");


            RuleFor(x => x.AppUserId)
                .GreaterThan(0).WithMessage("KullanıcıId sıfırdan büyük olmalıdır.");
            RuleFor(x => x.ItemTitle)
                .MinimumLength(0).WithMessage("Eşya isminin karakter sayısı sıfırdan büyük olmalıdır.")
                .MaximumLength(40).WithMessage("Eşya isminin karakter sayısı 40'dan küçük olmalıdır.");


        }
    }
}
