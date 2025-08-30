namespace Instapound.Web.Models;

public record FollowButtonViewModel(
    Guid Id,
    bool CurrentUserFollows);