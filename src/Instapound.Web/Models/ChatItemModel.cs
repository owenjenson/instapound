namespace Instapound.Web.Models;

public record ChatItemModel(
    SimpleUserViewModel User,
    string LatestMessage,
    string LatestMessageSenderName,
    DateTime LatestMessageSentAt);