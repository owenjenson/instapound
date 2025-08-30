using Instapound.Data;
using Instapound.Data.Entities;
using Instapound.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Instapound.Web.Services;

public class PostsService(AppDbContext dbContext)
{
    private readonly AppDbContext dbContext = dbContext;

    public async Task CommentPost(Guid postId, Guid userId, string text)
    {
        var postExists = await dbContext.Posts.AnyAsync((p) => p.Id == postId);
        var userExists = await dbContext.Users.AnyAsync((u) => u.Id == userId);

        if (postExists && userExists)
        {
            dbContext.Comments.Add(new Comment { PostId = postId, AuthorId = userId, Text = text, CreatedAt = DateTime.Now });
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<LikeButtonContentViewModel> LikePost(Guid postId, Guid userId)
    {
        var postExists = await dbContext.Posts.AnyAsync((p) => p.Id == postId);
        var userExists = await dbContext.Users.AnyAsync((u) => u.Id == userId);
        var isLiked = await dbContext.UserLikesPosts.AnyAsync((up) => up.UserId == userId && up.PostId == postId);

        if (postExists && userExists)
        {
            if (!isLiked)
            {
                dbContext.UserLikesPosts.Add(new UserLikesPost { PostId = postId, UserId = userId });
                await dbContext.SaveChangesAsync();
            }
            else
            {
                await dbContext.UserLikesPosts
                    .Where((up) => up.UserId == userId && up.PostId == postId)
                    .ExecuteDeleteAsync();
            }
        }

        return await dbContext.Posts
            .Include((p) => p.LikedByUsers)
            .Where((p) => p.Id == postId)
            .Select((p) => new LikeButtonContentViewModel(p.LikedByUsers.Count(), p.LikedByUsers.Any((u) => u.Id == userId)))
            .SingleAsync();
    }

    public async Task RemovePost(Guid postId, Guid userId)
    {
        var image = await dbContext.Posts
            .Where((p) => p.Id == postId && p.AuthorId == userId)
            .Select((p) => p.Image)
            .SingleOrDefaultAsync();

        if (image is not null)
        {
            var path = $"{Constants.PostImagesFolder}{image}";

            if (File.Exists(path))
                File.Delete(path);
        }

        await dbContext.Posts
            .Where((p) => p.Id == postId && p.AuthorId == userId)
            .ExecuteDeleteAsync();
    }

    public async Task UpdatePost(Guid postId, Guid userId, string text, string? image, bool changeImage = true)
    {
        var currentImage = await dbContext.Posts
            .Where((p) => p.Id == postId && p.AuthorId == userId)
            .Select((p) => p.Image)
            .SingleOrDefaultAsync();

        if (changeImage && currentImage != image && currentImage is not null)
        {
            var path = $"{Constants.PostImagesFolder}{currentImage}";

            if (File.Exists(path))
                File.Delete(path);
        }

        var newImage = changeImage ? image : currentImage;

        await dbContext.Posts
            .Where((p) => p.Id == postId && p.AuthorId == userId)
            .ExecuteUpdateAsync((setters) => setters
                .SetProperty((p) => p.Text, (p) => text)
                .SetProperty((p) => p.Image, (p) => newImage)
                .SetProperty((p) => p.IsEdited, (p) => true));
    }

    public async Task<bool> AnyPost(Guid postId, Guid userId)
    {
        return await dbContext.Posts
            .AnyAsync((p) => p.Id == postId && p.AuthorId == userId);
    }

    public async Task<EditPostViewModel?> GetPostToEdit(Guid postId, Guid currentUserId)
    {
        return await dbContext.Posts
            .Where((p) => p.Id == postId)
            .Select((p) => new EditPostViewModel { Text = p.Text, CurrentImage = p.Image })
            .SingleOrDefaultAsync();
    }

    public async Task<PostViewModel?> GetPost(Guid postId, Guid currentUserId)
    {
        return await dbContext.Posts
            .Include((p) => p.Author)
            .Include((p) => p.Comments)
            .Include((p) => p.LikedByUsers)
            .Where((p) => p.Id == postId)
            .Select((p) => new PostViewModel(
                new SimpleUserViewModel(
                    p.Author!.Id,
                    p.Author!.UserName!,
                    $"{p.Author!.FirstName} {p.Author!.LastName}",
                    p.Author!.Avatar),
                p.Id,
                p.ReleasedAt,
                p.Text,
                p.Image,
                p.LikedByUsers.Count(),
                p.Comments.Count(),
                p.AuthorId == currentUserId,
                p.LikedByUsers.Any((u) => u.Id == currentUserId),
                p.IsEdited))
            .SingleOrDefaultAsync();
    }

    public async Task<List<PostViewModel>> GetAllPosts(Guid currentUserId)
    {
        return await dbContext.Posts
            .Include((p) => p.Author)
            .Include((p) => p.Comments)
            .ThenInclude((c) => c.Author)
            .Include((p) => p.Comments)
            .ThenInclude((c) => c.LikedByUsers)
            .Include((p) => p.LikedByUsers)
            .OrderByDescending((p) => p.ReleasedAt)
            .Select((p) => new PostViewModel(
                new SimpleUserViewModel(
                    p.AuthorId,
                    p.Author!.UserName!,
                    $"{p.Author!.FirstName} {p.Author!.LastName}",
                    p.Author!.Avatar),
                p.Id,
                p.ReleasedAt,
                p.Text,
                p.Image,
                p.LikedByUsers.Count(),
                p.Comments.Count(),
                p.AuthorId == currentUserId,
                p.LikedByUsers.Any((u) => u.Id == currentUserId),
                p.IsEdited))
            .ToListAsync();
    }

    public async Task<List<PostViewModel>> GetPostsByAuthorId(Guid authorId, Guid currentUserId)
    {
        return await dbContext.Posts
            .Include((p) => p.Author)
            .Include((p) => p.Comments)
            .ThenInclude((c) => c.Author)
            .Include((p) => p.Comments)
            .ThenInclude((c) => c.LikedByUsers)
            .Include((p) => p.LikedByUsers)
            .Where((p) => p.AuthorId == authorId)
            .OrderByDescending((p) => p.ReleasedAt)
            .Select((p) => new PostViewModel(
                new SimpleUserViewModel(
                    p.AuthorId,
                    p.Author!.UserName!,
                    $"{p.Author!.FirstName} {p.Author!.LastName}",
                    p.Author!.Avatar),
                p.Id,
                p.ReleasedAt,
                p.Text,
                p.Image,
                p.LikedByUsers.Count(),
                p.Comments.Count(),
                p.AuthorId == currentUserId,
                p.LikedByUsers.Any((u) => u.Id == currentUserId),
                p.IsEdited))
            .ToListAsync();
    }

    public async Task<Guid> InsertPost(string text, string? image, Guid authorId)
    {
        var post = new Post
        {
            ReleasedAt = DateTime.Now,
            Text = text,
            Image = image,
            AuthorId = authorId,
        };

        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync();

        return post.Id;
    }
}