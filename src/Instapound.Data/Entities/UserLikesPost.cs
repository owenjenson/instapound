using Microsoft.EntityFrameworkCore;

namespace Instapound.Data.Entities;

[PrimaryKey(nameof(UserId), nameof(PostId))]
public class UserLikesPost
{
    public required Guid UserId { get; set; }
    public required Guid PostId { get; set; }

    public User? User { get; set; }
    public Post? Post { get; set; }
}