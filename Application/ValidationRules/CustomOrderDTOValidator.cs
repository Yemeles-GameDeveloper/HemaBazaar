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
    internal class CustomOrderDTOValidator : AbstractValidator<CustomOrderDTO>
    {
        public CustomOrderDTOValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("SiparişId boş bırakılamaz");
            RuleFor(x => x.BandColor)
                .NotEmpty()
                .MaximumLength(30).WithMessage("Bandaj renk ismi 30 karakterden fazla olamaz.");
            RuleFor(x => x.ThreadColor)
                .NotEmpty()
                .MaximumLength(30).WithMessage("İplik renk ismi 30 karakterden fazla olamaz.");
            RuleFor(x => x.LaceColor)
                .NotEmpty()
                .MaximumLength(30).WithMessage("Bağcık renk ismi 30 karakterden fazla olamaz.");
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Kategori boş geçilemez");
            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model kısmı boş geçilemez");
            RuleFor(x => x.Size)
                .NotEmpty().WithMessage("Boy boş geçilemez");


        }
    }
}
