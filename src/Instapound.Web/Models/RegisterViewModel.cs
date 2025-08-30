using System.ComponentModel.DataAnnotations;

namespace Instapound.Web.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Enter your username")]
    [RegularExpression("^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can contain only alphanumeric characters")]
    [MinLength(3, ErrorMessage = "Username must have at least 3 characters")]
    [Display(Name = "Username")]
    public required string UserName { get; set; }

    [Required(ErrorMessage = "Enter your password")]
    [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Confirm your password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "Entered passwords have to match")]
    public required string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Enter your first name")]
    [RegularExpression("^[a-zA-ZáčďéěíňóřšťůúýžÁČĎÉĚÍŇÓŘŠŤŮÚÝŽ0-9]+$", ErrorMessage = "First name can contain only alphanumeric characters")]
    [Display(Name = "First name")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Enter your last name")]
    [RegularExpression("^[a-zA-ZáčďéěíňóřšťůúýžÁČĎÉĚÍŇÓŘŠŤŮÚÝŽ0-9]+$", ErrorMessage = "Last name can contain only alphanumeric characters")]
    [Display(Name = "Last name")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Enter your date of birth")]
    [Display(Name = "Date of birth")]
    [DataType(DataType.Date)]
    public required DateTime DateOfBirth { get; set; }

    [Display(Name = "Bio")]
    public string? Bio { get; set; }

    [Display(Name = "Profile picture")]
    public IFormFile? Avatar { get; set; }
}