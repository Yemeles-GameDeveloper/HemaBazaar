using Domain.Entities;
using FluentValidation;
using HemaBazaar.MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ValidationRules
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        UserManager<AppUser> _userManager;

        public RegisterViewModelValidator(UserManager<AppUser> userManager)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-mail is required.")
                .EmailAddress().WithMessage("Please enter a valid e-mail")
                .Must(CheckEmail).WithMessage("E-mail is already existed");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password cannot be shorter than 6 characters.")
                //.Must(ContainUppercase).WithMessage("Password must contain at least one upper-case character.")
                //.Must(ContainLowercase).WithMessage("Password must contain at least one lower-case charahter")
                //.Must(ContainDigit).WithMessage("Password must contain at least one numerical character")
                //.Must(ContainSpecialCharacter).WithMessage("Password must contain at least one special character.");
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$")
                .WithMessage("Password must contain upper, lower, digit and special character.");



            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .Must(CheckUsername).WithMessage("Username is already existed.");
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Fullname is required.");
            this._userManager = userManager;
        }

        private bool ContainUppercase(string password) => password?.Any(char.IsUpper) == true;
        private bool ContainLowercase(string password) => password?.Any(char.IsLower) == true;
        private bool ContainDigit(string password) => password?.Any(char.IsDigit) == true;
        private bool ContainSpecialCharacter(string password) => password?.Any(ch=>!char.IsLetterOrDigit(ch)) == true;

         bool CheckUsername(string userName)
        {
            return !_userManager.Users.Any(x=> x.UserName == userName);
        }

         bool CheckEmail(string eMail)
        {
            return !_userManager.Users.Any(x => x.Email == eMail);
        }

        
    }
}
