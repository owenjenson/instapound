using Instapound.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Instapound.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<UserLikesPost> UserLikesPosts { get; set; } = null!;
    public DbSet<UserLikesComment> UserLikesComments { get; set; } = null!;
    public DbSet<UserFollowsUser> UserFollowsUsers { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Following)
            .WithMany(u => u.FollowedBy)
            .UsingEntity<UserFollowsUser>(
                l => l.HasOne((j) => j.FollowedUser).WithMany((a) => a.FollowingJoins).HasForeignKey((j) => j.FollowedUserId),
                r => r.HasOne((j) => j.FollowedByUser).WithMany((s) => s.FollowedByJoins).HasForeignKey((j) => j.FollowedByUserId));

        modelBuilder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.Author);

        modelBuilder.Entity<User>()
            .HasMany(u => u.LikedPosts)
            .WithMany(p => p.LikedByUsers)
            .UsingEntity<UserLikesPost>(
                l => l.HasOne((j) => j.Post).WithMany((a) => a.LikedByUsersJoins).HasForeignKey((j) => j.PostId),
                r => r.HasOne((j) => j.User).WithMany((s) => s.LikedPostsJoins).HasForeignKey((j) => j.UserId));

        modelBuilder.Entity<User>()
            .HasMany(u => u.TaggedAtPosts)
            .WithMany(p => p.TaggedUsers)
            .UsingEntity(join => join.ToTable("UserIsTaggedAtPost"));

        modelBuilder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(c => c.Author);

        modelBuilder.Entity<User>()
            .HasMany(u => u.LikedComments)
            .WithMany(c => c.LikedByUsers)
            .UsingEntity<UserLikesComment>(
                l => l.HasOne((j) => j.Comment).WithMany((a) => a.LikedByUsersJoins).HasForeignKey((j) => j.CommentId),
                r => r.HasOne((j) => j.User).WithMany((s) => s.LikedCommentsJoins).HasForeignKey((j) => j.UserId));

        modelBuilder.Entity<User>()
            .HasMany(u => u.SentMessages)
            .WithOne(m => m.FromUser);

        modelBuilder.Entity<User>()
            .HasMany(u => u.ReceivedMessages)
            .WithOne(m => m.ToUser);

        modelBuilder.Entity<Post>()
            .Property((p) => p.IsEdited)
            .HasDefaultValue(false);
    }
}