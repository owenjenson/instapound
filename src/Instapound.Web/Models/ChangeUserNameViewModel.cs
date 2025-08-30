using System.ComponentModel.DataAnnotations;

namespace Instapound.Web.Models;

public class ChangeUserNameViewModel
{
    [Required(ErrorMessage = "Enter your password")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Enter your new username")]
    [RegularExpression("^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can contain only alphanumeric characters")]
    [MinLength(3, ErrorMessage = "Username must have at least 3 characters")]
    [Display(Name = "New username")]
    public required string UserName { get; set; }

    public bool IsSuccess { get; init; } = false;
}