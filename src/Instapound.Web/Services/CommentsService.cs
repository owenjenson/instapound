using Instapound.Data;
using Instapound.Data.Entities;
using Instapound.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Instapound.Web.Services;

public class CommentsService(AppDbContext dbContext)
{
    private readonly AppDbContext dbContext = dbContext;

    public async Task<Guid> RemoveComment(Guid commentId, Guid userId)
    {
        var postId = await dbContext.Comments
            .Where((c) => c.Id == commentId && c.AuthorId == userId)
            .Select((c) => c.PostId)
            .SingleAsync();

        await dbContext.Comments
            .Where((c) => c.Id == commentId && c.AuthorId == userId)
            .ExecuteDeleteAsync();

        return postId;
    }

    public async Task<LikeButtonContentViewModel> LikeComment(Guid commentId, Guid userId)
    {
        var commentExists = await dbContext.Comments.AnyAsync((p) => p.Id == commentId);
        var userExists = await dbContext.Users.AnyAsync((p) => p.Id == userId);
        var isLiked = await dbContext.UserLikesComments.AnyAsync((up) => up.UserId == userId && up.CommentId == commentId);

        if (commentExists && userExists)
        {
            if (!isLiked)
            {
                dbContext.UserLikesComments.Add(new UserLikesComment { CommentId = commentId, UserId = userId });
                await dbContext.SaveChangesAsync();
            }
            else
            {
                await dbContext.UserLikesComments
                    .Where((up) => up.UserId == userId && up.CommentId == commentId)
                    .ExecuteDeleteAsync();
            }
        }

        return await dbContext.Comments
            .Include((p) => p.LikedByUsers)
            .Where((p) => p.Id == commentId)
            .Select((p) => new LikeButtonContentViewModel(p.LikedByUsers.Count(), p.LikedByUsers.Any((u) => u.Id == userId)))
            .SingleAsync();
    }

    public async Task<List<CommentViewModel>> GetPostComments(Guid postId, Guid currentUserId)
    {
        return await dbContext.Comments
            .Include((c) => c.LikedByUsers)
            .Include((c) => c.Author)
            .Where((c) => c.PostId == postId)
            .OrderByDescending((c) => c.CreatedAt)
            .Select((c) => new CommentViewModel(
                new SimpleUserViewModel(
                    c.Author!.Id,
                    c.Author!.UserName!,
                    $"{c.Author!.FirstName} {c.Author!.LastName}",
                    c.Author!.Avatar),
                c.Id,
                c.Text,
                c.LikedByUsers.Count(),
                c.AuthorId == currentUserId,
                c.LikedByUsers.Any((u) => u.Id == currentUserId),
                c.CreatedAt))
            .ToListAsync();
    }
}