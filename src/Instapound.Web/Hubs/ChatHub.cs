using Instapound.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Instapound.Web.Hubs;

public class ChatHub(UserManager<User> userManager) : Hub
{
    private readonly UserManager<User> userManager = userManager;

    public override async Task OnConnectedAsync()
    {
        var user = await userManager.GetUserAsync(Context.User!) ?? throw new NullReferenceException("User cannot be null");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{user.Id.ToString().ToLower()}");
        await base.OnConnectedAsync();
    }

    public async Task NotifyUser(string userId)
    {
        var user = await userManager.GetUserAsync(Context.User!) ?? throw new NullReferenceException("User cannot be null");
        var group = $"user_{Guid.Parse(userId).ToString().ToLower()}";

        await Clients.Group(group).SendAsync("ReceivedMessageFromUser", user.Id.ToString());
    }
}