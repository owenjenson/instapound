namespace Instapound.Web.Models;

public record ChatMessageViewModel(
    Guid SenderId,
    string SenderName,
    string? SenderAvatar,
    string Text,
    DateTime SentAt,
    bool SentByCurrentUser);