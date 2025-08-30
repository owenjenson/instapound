using Instapound.Data;
using Instapound.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Instapound.Web.Services;

public class MessagesService(AppDbContext dbContext)
{
    private readonly AppDbContext dbContext = dbContext;

    public async Task InsertMessage(string text, Guid fromUserId, Guid toUserId)
    {
        dbContext.Messages.Add(new Data.Entities.Message
        {
            Text = text,
            CreatedAt = DateTime.Now,
            FromUserId = fromUserId,
            ToUserId = toUserId,
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task<List<ChatMessageViewModel>> GetChatMessages(Guid userId, Guid currentUserId, DateTime? since = null)
    {
        var query = dbContext.Messages
            .Include((m) => m.FromUser)
            .Where((m) => (m.FromUserId == userId && m.ToUserId == currentUserId) ||
                (m.ToUserId == userId && m.FromUserId == currentUserId));

        if (since is DateTime sinceDate)
            query = query.Where((m) => m.CreatedAt > sinceDate);

        return await query
            .OrderByDescending((m) => m.CreatedAt)
            .Select((m) => new ChatMessageViewModel(
                m.FromUserId,
                $"{m.FromUser!.FirstName} {m.FromUser!.LastName}",
                m.FromUser.Avatar,
                m.Text,
                m.CreatedAt,
                m.FromUserId == currentUserId))
            .ToListAsync();
    }

    public async Task<List<ChatItemModel>> GetChats(Guid userId)
    {
        var receivedLatestMessages = await dbContext.Messages
            .Where((m) => m.ToUserId == userId)
            .GroupBy(m => m.FromUserId)
            .Select((g) => g.OrderByDescending((m) => m.CreatedAt).First())
            .ToListAsync();

        var sentLatestMessages = await dbContext.Messages
            .Where((m) => m.FromUserId == userId)
            .GroupBy(m => m.ToUserId)
            .Select((g) => g.OrderByDescending((m) => m.CreatedAt).First())
            .ToListAsync();

        var chatMessages = new List<ChatItemModel>();

        var userIdsSet = new HashSet<Guid>();

        foreach (var messages in receivedLatestMessages.Concat(sentLatestMessages))
        {
            userIdsSet.Add(messages.ToUserId);
            userIdsSet.Add(messages.FromUserId);
        }

        var userIds = userIdsSet.ToArray();
        var users = await dbContext.Users
            .Where((u) => userIds.Contains(u.Id))
            .Select((u) => new SimpleUserViewModel(
                u.Id,
                u.UserName!,
                $"{u.FirstName} {u.LastName}",
                u.Avatar))
            .ToListAsync();

        var isSameUserChatAdded = false;

        foreach (var message in receivedLatestMessages.Concat(sentLatestMessages).OrderByDescending((m) => m.CreatedAt))
        {
            var isTheSameUserChat = message.ToUserId == message.FromUserId && message.ToUserId == userId;
            var alreadyContainsChat = chatMessages.Any((m) =>
                    (message.ToUserId == m.User.Id || message.FromUserId == m.User.Id) && m.User.Id != userId);

            if ((!alreadyContainsChat && !isTheSameUserChat) || (!alreadyContainsChat && isTheSameUserChat && !isSameUserChatAdded))
            {
                chatMessages.Add(new ChatItemModel(
                    message.FromUserId == userId ?
                        users.First((u) => u.Id == message.ToUserId) :
                        users.First((u) => u.Id == message.FromUserId),
                    message.Text,
                    users.First((u) => u.Id == message.FromUserId).Name,
                    message.CreatedAt));

                if (isTheSameUserChat)
                    isSameUserChatAdded = true;
            }
        }

        return chatMessages;
    }
}