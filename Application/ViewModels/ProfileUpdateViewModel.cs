using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class ProfileUpdateViewModel :IValidatableObject
    {
        [Required(ErrorMessage ="Fullname is required.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Fullname is required.")]
        public string UserName { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        [EmailAddress(ErrorMessage ="Enter a valid email address.")]
        [Required(ErrorMessage = "Fullname is required.")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        
        [DataType(DataType.Password)]
        
        public string? ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool wantsToChangePassword =
                !string.IsNullOrWhiteSpace(CurrentPassword) ||
                !string.IsNullOrWhiteSpace(NewPassword) ||
                !string.IsNullOrWhiteSpace(ConfirmPassword);

            if (!wantsToChangePassword)
                yield break;

            if (string.IsNullOrWhiteSpace(CurrentPassword))
                yield return new ValidationResult(
                    "Current password is required to change your password.",
                    new[] { nameof(CurrentPassword) });

            if (string.IsNullOrWhiteSpace(NewPassword))
                yield return new ValidationResult(
                    "New password is required to change your password.",
                    new[] { nameof(NewPassword) });

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
                yield return new ValidationResult(
                    "Password confirmation is required when updating your password.",
                    new[] { nameof(ConfirmPassword) });

            if (!string.Equals(NewPassword, ConfirmPassword))
                yield return new ValidationResult(
                    "Passwords are not matching.",
                    new[] { nameof(ConfirmPassword) });
        }

    }
}
