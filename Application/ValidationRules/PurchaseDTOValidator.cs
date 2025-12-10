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
    internal class PurchaseDTOValidator: AbstractValidator<PurchaseDTO>
    {
        IUnitOfWork UnitOfWork;

        public PurchaseDTOValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.AppUserId)
                .NotEmpty().WithMessage("AppUserId cannot be empty.");
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username cannot be empty.");
            RuleFor(x => x.ItemTitle)
                .NotEmpty().WithMessage("Item title cannot be empty.");
            UnitOfWork = unitOfWork;
        }
        
    }
}
