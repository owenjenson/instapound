using System.ComponentModel.DataAnnotations;

namespace Instapound.Web.Models;

public class ChangeProfileInfoViewModel
{
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

    public bool IsSuccess { get; init; } = false;
}