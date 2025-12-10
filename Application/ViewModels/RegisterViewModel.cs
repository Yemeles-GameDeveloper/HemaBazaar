using System.ComponentModel.DataAnnotations;

namespace HemaBazaar.MVC.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Fullname is required.")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "E-mail is required.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail should be valid.")]
        public string Email { get; set; }


        [Required(ErrorMessage ="Username is required.")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password, ErrorMessage = "Password should be valid.")]       
        [MinLength(6,ErrorMessage ="Password should be at least 6 character lenght.")]

        
        public string Password { get; set; }


        [Required(ErrorMessage = "Password confirmation is required.")]
        [DataType(DataType.Password)] 
        [Compare("Password", ErrorMessage ="Passwords are not matching.")]
        public string ConfirmPassword { get; set; }


        // 20 Kasım 2:13:14 devam et.
    }
}
