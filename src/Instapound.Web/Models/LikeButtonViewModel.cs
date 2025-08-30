namespace Instapound.Web.Models;

public record LikeButtonViewModel(
    int LikesCount,
    Guid Id,
    string Url,
    bool CurrentUserLikes);