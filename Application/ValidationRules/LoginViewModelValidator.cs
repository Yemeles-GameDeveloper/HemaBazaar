using Domain.Entities;
using FluentValidation;
using HemaBazaar.MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Application.ValidationRules
{
    
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        UserManager<AppUser> _userManager;
        public LoginViewModelValidator(UserManager<AppUser> userManager)
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username cannot be empty")
                .Must(CheckUserName).WithMessage("No user with this username was found.")
                .Must(CheckEmailConfirmed).WithMessage("E-mail must be validated.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password cannot be empty")
                .MinimumLength(6).WithMessage("Password must be at least 6 character lenght.");

            //RuleFor(x => x)
            //    .MustAsync(async(model, ct) => CheckPassword(model.UserName, model.Password)).WithMessage("Password is not correct.");

            RuleFor(x => x)
                .Must((model, password)=> CheckPassword(model.UserName, model.Password))
                .WithMessage("Username or password is false");

            _userManager = userManager;
        }

        bool CheckUserName (string userName) => _userManager.Users.Any (x => x.UserName == userName);

        bool CheckEmailConfirmed(string userName)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.UserName == userName);
            if (user == null)
            {
                return false;
            }

            return user.EmailConfirmed;
        }

          bool CheckPassword(string username, string password)
        {
            AppUser user = _userManager.FindByNameAsync(username).Result;
            bool result = _userManager.CheckPasswordAsync(user, password).Result;
            return result;
        }

        //20 Kasım 3:03 den devam et.
    }
}
