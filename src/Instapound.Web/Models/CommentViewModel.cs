namespace Instapound.Web.Models;

public record CommentViewModel(
    SimpleUserViewModel Author,
    Guid Id,
    string Text,
    int LikesCount,
    bool CurrentUserIsAuthor,
    bool CurrentUserLikes,
    DateTime CreatedAt);