using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ValidationRules 
{
    internal class ItemValidator : AbstractValidator<Item>
    {
        public ItemValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş olamaz.")
                .MinimumLength(1).WithMessage("Başlık tek karakterden kısa olamaz.")
                .MaximumLength(120).WithMessage("Başlık en fazla 120 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama kısmı boş olamaz.")
                .MinimumLength(10).WithMessage("Açıklama 10 karakterden kısa olamaz");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Fiyat negatif olamaz.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("İçerik kısmı boş olamaz");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Ürünün mutlaka kategorisi olmak zorundadır.");

        }
    }
}
