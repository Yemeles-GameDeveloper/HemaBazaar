using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "E-mail is required.")]
        [EmailAddress(ErrorMessage = "Please, enter a valid e-mail address.")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Token is required")]
        public string Token { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password, ErrorMessage = "Password should be valid.")]
        [MinLength(6, ErrorMessage = "Password should be at least 6 character lenght.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Password confirmation is required.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords are not matching.")]
        public string ConfirmPassword { get; set; }


    }
}
