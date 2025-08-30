using Microsoft.AspNetCore.Identity;

namespace Instapound.Data.Entities;

public class User : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public string? Bio { get; set; }
    public string? Avatar { get; set; }

    public List<User> Following { get; } = [];
    public List<UserFollowsUser> FollowingJoins { get; } = [];
    public List<User> FollowedBy { get; } = [];
    public List<UserFollowsUser> FollowedByJoins { get; } = [];

    public List<Post> Posts { get; } = [];
    public List<Post> TaggedAtPosts { get; } = [];
    public List<Post> LikedPosts { get; } = [];
    public List<UserLikesPost> LikedPostsJoins { get; } = [];

    public List<Comment> Comments { get; } = [];
    public List<Comment> LikedComments { get; } = [];
    public List<UserLikesComment> LikedCommentsJoins { get; } = [];

    public List<Message> SentMessages { get; } = [];
    public List<Message> ReceivedMessages { get; } = [];
}