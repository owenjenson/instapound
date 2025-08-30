using Microsoft.EntityFrameworkCore;

namespace Instapound.Data.Entities;

[PrimaryKey(nameof(UserId), nameof(CommentId))]
public class UserLikesComment
{
    public required Guid UserId { get; set; }
    public required Guid CommentId { get; set; }

    public User? User { get; set; }
    public Comment? Comment { get; set; }
}