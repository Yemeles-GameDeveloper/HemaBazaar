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
    internal class OrderDetailDTOValidatior : AbstractValidator<OrderDetailDTO>
    {
        public OrderDetailDTOValidatior()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("SiparişId boş geçilemez.");
            RuleFor(x => x.Item)
                .NotEmpty().WithMessage("Eşya boş geçilemez.");
            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Fiyat negatif olamaz");
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Miktar negatif olamaz");
        }
    }
}
