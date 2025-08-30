using System.ComponentModel.DataAnnotations;

namespace Instapound.Web.Models;

public class EditPostViewModel
{
    [Required(ErrorMessage = "Enter content of the post")]
    [MinLength(1, ErrorMessage = "Content must have at least 1 character")]
    [Display(Name = "Content")]
    public required string Text { get; set; }

    public string? CurrentImage { get; set; }

    public bool ChangedImage { get; set; } = false;

    [Display(Name = "Image")]
    public IFormFile? NewImage { get; set; }
}