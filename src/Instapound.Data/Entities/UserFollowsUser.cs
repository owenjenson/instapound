using Microsoft.EntityFrameworkCore;

namespace Instapound.Data.Entities;

[PrimaryKey(nameof(FollowedUserId), nameof(FollowedByUserId))]
public class UserFollowsUser
{
    public required Guid FollowedUserId { get; set; }
    public required Guid FollowedByUserId { get; set; }

    public User? FollowedUser { get; set; }
    public User? FollowedByUser { get; set; }
}