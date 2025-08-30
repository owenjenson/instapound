using System.ComponentModel.DataAnnotations;

namespace Instapound.Web.Models;

public class NewCommentViewModel
{
    [Required]
    public required string PostId { get; set; }
    [Required(ErrorMessage = "Enter the comment")]
    [MinLength(1, ErrorMessage = "Comment must contain at least 1 character")]
    public required string Text { get; set; }
}