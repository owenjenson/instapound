namespace Instapound.Web.Models;

public record LikeButtonContentViewModel(
    int LikesCount,
    bool CurrentUserLikes);