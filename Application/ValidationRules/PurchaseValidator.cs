using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ValidationRules
{
    internal class PurchaseValidator: AbstractValidator<Purchase>
    {
        IUnitOfWork UnitOfWork;

        public PurchaseValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.AppUserId)
                .NotEmpty().WithMessage("AppUserId cannot be empty.");
            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage("ItemId cannot be empty.");
            RuleFor(x => x.PaymentId)
                .NotEmpty().WithMessage("PaymentId title cannot be empty.");
            UnitOfWork = unitOfWork;
        }
        
    }
}
