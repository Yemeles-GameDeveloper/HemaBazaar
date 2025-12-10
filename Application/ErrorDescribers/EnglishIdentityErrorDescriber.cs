using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ErrorDescribers
{
    public class EnglishIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new() { Code = "PasswordTooShort", Description = $"The password should be at least {length} lenght." };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new() { Code = "PasswordRequiresUpper", Description = $"The password should contain at least one upper-case character." };
        }

        public override IdentityError PasswordRequiresLower() => new IdentityError
        { Code = "PasswordRequiresLower", Description = $"The password should contain at least one lower-case character." };


        public override IdentityError PasswordRequiresDigit() => new IdentityError
        { Code = "PasswordRequiresDigit", Description = $"The password should contain at least one numerical character." };

        public override IdentityError PasswordRequiresNonAlphanumeric() => new IdentityError
        { Code = "PasswordRequiresNonAlphanumeric", Description = $"The password should contain at least one special  character." };

    }
}
