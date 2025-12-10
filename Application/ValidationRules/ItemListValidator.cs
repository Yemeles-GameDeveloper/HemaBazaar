using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ValidationRules
{
    public class ItemListValidator : AbstractValidator<IEnumerable<ItemDTO>>
    {
        public ItemListValidator()
        {
            RuleForEach(x => x).SetValidator(new ItemDTOValidator());
        }
    }
}
