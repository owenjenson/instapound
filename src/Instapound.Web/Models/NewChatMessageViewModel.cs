using System.ComponentModel.DataAnnotations;

namespace Instapound.Web.Models;

public class NewChatMessageViewModel
{
    [Required(ErrorMessage = "Enter the message")]
    public required string Text { get; set; }
    public long? LatestMessageTime { get; set; }
}