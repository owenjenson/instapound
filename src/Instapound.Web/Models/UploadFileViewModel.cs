using System.ComponentModel.DataAnnotations;

namespace Instapound.Web.Models;

public class UploadFileViewModel
{
    [Required(ErrorMessage = "Choose a picture")]
    public IFormFile? File { get; set; }
}