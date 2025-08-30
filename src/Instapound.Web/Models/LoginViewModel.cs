using System.ComponentModel.DataAnnotations;

namespace Instapound.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Enter your username")]
    [Display(Name = "Username")]
    public required string UserName { get; set; }

    [Required(ErrorMessage = "Enter your password")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; set; }

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}