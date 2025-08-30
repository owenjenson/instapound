using Instapound.Data;
using Instapound.Data.Entities;
using Instapound.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Instapound.Web.Services;

public class UsersService(UserManager<User> userManager, AppDbContext dbContext)
{
    private readonly UserManager<User> userManager = userManager;
    private readonly AppDbContext dbContext = dbContext;

    public async Task<FollowButtonContentViewModel> FollowUser(Guid followedUserId, Guid currentUserId)
    {
        var followedUserExists = await dbContext.Users.AnyAsync((u) => u.Id == followedUserId);
        var currentUserExists = await dbContext.Users.AnyAsync((u) => u.Id == currentUserId);
        var isFollowed = await dbContext.UserFollowsUsers.AnyAsync((uu) => uu.FollowedByUserId == currentUserId && uu.FollowedUserId == followedUserId);

        if (followedUserExists && currentUserExists)
        {
            if (!isFollowed)
            {
                dbContext.UserFollowsUsers.Add(new UserFollowsUser { FollowedUserId = followedUserId, FollowedByUserId = currentUserId });
                await dbContext.SaveChangesAsync();
            }
            else
            {
                await dbContext.UserFollowsUsers
                    .Where((uu) => uu.FollowedByUserId == currentUserId && uu.FollowedUserId == followedUserId)
                    .ExecuteDeleteAsync();
            }
        }

        return await dbContext.Users
            .Include((u) => u.FollowedBy)
            .Where((u) => u.Id == followedUserId)
            .Select((u) => new FollowButtonContentViewModel(u.FollowedBy.Any((u) => u.Id == currentUserId)))
            .SingleAsync();
    }

    public async Task<bool> UpdateUserName(User user, string newUserName)
    {
        var normalizedUserName = userManager.NormalizeName(newUserName);
        var alreadyExists = await dbContext.Users.AnyAsync((u) => u.NormalizedUserName == normalizedUserName);

        if (alreadyExists)
            return false;

        await userManager.SetUserNameAsync(user, newUserName);
        await userManager.UpdateNormalizedUserNameAsync(user);

        return true;
    }

    public async Task<ProfileInfoViewModel?> GetProfileInfo(Guid userId, Guid currentUserId)
    {
        return await dbContext.Users
            .Include((u) => u.FollowedBy)
            .Include((u) => u.Following)
            .Include((u) => u.Posts)
            .Where((u) => u.Id == userId)
            .Select((u) => new ProfileInfoViewModel(
                u.Id,
                u.UserName!,
                $"{u.FirstName} {u.LastName}",
                u.FollowedBy.Count(),
                u.Following.Count(),
                u.Posts.Count(),
                u.FollowedBy.Any((f) => f.Id == currentUserId),
                u.Avatar,
                u.Bio))
            .SingleOrDefaultAsync();
    }

    public async Task<SimpleUserViewModel?> GetSimpleProfileInfo(Guid userId)
    {
        return await dbContext.Users
            .Where((u) => u.Id == userId)
            .Select((u) => new SimpleUserViewModel(
                u.Id,
                u.UserName!,
                $"{u.FirstName} {u.LastName}",
                u.Avatar))
            .SingleOrDefaultAsync();
    }

    public async Task<List<ProfileInfoViewModel>> GetFollowers(Guid userId, Guid currentUserId)
    {
        return await dbContext.Users
            .Include((u) => u.FollowedBy)
            .Include((u) => u.Following)
            .Include((u) => u.Posts)
            .Where((u) => u.Following.Any((u) => u.Id == userId))
            .Select((u) => new ProfileInfoViewModel(
                u.Id,
                u.UserName!,
                $"{u.FirstName} {u.LastName}",
                u.FollowedBy.Count(),
                u.Following.Count(),
                u.Posts.Count(),
                u.FollowedBy.Any((f) => f.Id == currentUserId),
                u.Avatar,
                u.Bio))
            .ToListAsync();
    }

    public async Task<List<ProfileInfoViewModel>> GetFollows(Guid userId, Guid currentUserId)
    {
        return await dbContext.Users
            .Include((u) => u.FollowedBy)
            .Include((u) => u.Following)
            .Include((u) => u.Posts)
            .Where((u) => u.FollowedBy.Any((u) => u.Id == userId))
            .Select((u) => new ProfileInfoViewModel(
                u.Id,
                u.UserName!,
                $"{u.FirstName} {u.LastName}",
                u.FollowedBy.Count(),
                u.Following.Count(),
                u.Posts.Count(),
                u.FollowedBy.Any((f) => f.Id == currentUserId),
                u.Avatar,
                u.Bio))
            .ToListAsync();
    }
}