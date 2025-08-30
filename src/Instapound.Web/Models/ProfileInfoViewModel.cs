namespace Instapound.Web.Models;

public record ProfileInfoViewModel(
    Guid Id,
    string UserName,
    string Name,
    int FollowersCount,
    int FollowingCount,
    int PostsCount,
    bool CurrentUserFollows,
    string? Avatar,
    string? Bio);