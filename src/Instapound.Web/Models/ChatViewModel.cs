namespace Instapound.Web.Models;

public record ChatViewModel(
    SimpleUserViewModel User,
    List<ChatMessageViewModel> Messages);