namespace Instapound.Data.Entities;

public class Post
{
    public Guid Id { get; set; }
    public required DateTime ReleasedAt { get; set; }
    public required string Text { get; set; }
    public string? Image { get; set; }
    public bool IsEdited { get; set; } = false;

    public List<Comment> Comments { get; } = [];

    public List<User> TaggedUsers { get; } = [];
    public List<User> LikedByUsers { get; } = [];
    public List<UserLikesPost> LikedByUsersJoins { get; } = [];

    public Guid AuthorId { get; set; }
    public User? Author { get; set; }
}