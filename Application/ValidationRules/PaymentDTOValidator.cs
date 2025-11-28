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
    internal class PaymentDTOValidator : AbstractValidator<PaymentDTO>
    {
        IUnitOfWork UnitOfWork;

        public PaymentDTOValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Miktar sıfırdan büyük olmalıdır");

            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("İşlem numarası boş geçilemez")
                .MustAsync(TransactionIdIsUnique).WithMessage("İşlem numarası daha önce kayıt edilmiştir. ");
            UnitOfWork = unitOfWork;
        }
        async Task<bool> TransactionIdIsUnique(string transactionId,CancellationToken cancellationToken)
        {
            IEnumerable<Payment> result = await UnitOfWork.Payments.FindAsync(x => x.TransactionId == transactionId);
            if (result.Count() > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
