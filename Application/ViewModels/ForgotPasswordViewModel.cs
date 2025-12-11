using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage ="E-mail is required.")]
        [EmailAddress(ErrorMessage ="Please, enter a valid e-mail address.")]
        public string Email { get; set; }

    }
}
