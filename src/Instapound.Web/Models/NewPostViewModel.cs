using System.ComponentModel.DataAnnotations;

namespace Instapound.Web.Models;

public class NewPostViewModel
{
    [Required(ErrorMessage = "Enter content of the post")]
    [MinLength(1, ErrorMessage = "Content must have at least 1 character")]
    [Display(Name = "Content")]
    public required string Text { get; set; }

    [Display(Name = "Image")]
    public IFormFile? Image { get; set; }
}