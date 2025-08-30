using System.ComponentModel.DataAnnotations;

namespace Instapound.Web.Models;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Enter your current password")]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public required string CurrentPassword { get; set; }

    [Required(ErrorMessage = "Enter your new password")]
    [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public required string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm your new password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare(nameof(NewPassword), ErrorMessage = "Entered passwords have to match")]
    public required string ConfirmNewPassword { get; set; }

    public bool IsSuccess { get; init; } = false;
}