namespace Instapound.Web.Models;

public record PostViewModel(
    SimpleUserViewModel Author,
    Guid Id,
    DateTime ReleasedAt,
    string Text,
    string? Image,
    int LikesCount,
    int CommentsCount,
    bool CurrentUserIsAuthor,
    bool CurrentUserLikes,
    bool IsEdited);