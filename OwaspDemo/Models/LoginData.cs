using System.ComponentModel.DataAnnotations;

namespace OwaspDemo.Models
{
    public class LoginData
    {
        [Required]
        [Display(Name = "User Name (email)")]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}