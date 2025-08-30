namespace Instapound.Data.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public required string Text { get; set; }
    public required DateTime CreatedAt { get; set; }

    public Guid PostId { get; set; }
    public Post? Post { get; set; }

    public Guid AuthorId { get; set; }
    public User? Author { get; set; }

    public List<User> LikedByUsers { get; } = [];
    public List<UserLikesComment> LikedByUsersJoins { get; } = [];
}