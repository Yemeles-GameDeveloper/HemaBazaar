using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class ProfileUpdateViewModel
    {
        [Required(ErrorMessage ="Fullname is required.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Fullname is required.")]
        public string UserName { get; set; }

        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        [EmailAddress(ErrorMessage ="Enter a valid email address.")]
        [Required(ErrorMessage = "Fullname is required.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Password confirmation is required.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords are not matching.")]
        public string ConfirmPassword { get; set; }

    }
}
