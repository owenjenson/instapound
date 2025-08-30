using Instapound.Data.Entities;
using Instapound.Web.Models;
using Instapound.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Instapound.Web.Controllers;

[Authorize]
public class MessagesController(UserManager<User> userManager, MessagesService messagesService, UsersService usersService) : Controller
{
    private readonly UserManager<User> userManager = userManager;
    private readonly MessagesService messagesService = messagesService;
    private readonly UsersService usersService = usersService;

    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var chats = await messagesService.GetChats(user.Id);

        return View(chats);
    }

    public async Task<IActionResult> Chat(string id)
    {
        if (!Guid.TryParse(id, out Guid userId))
            return NotFound();

        var currentUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var messages = await messagesService.GetChatMessages(userId, currentUser.Id);
        var user = await usersService.GetSimpleProfileInfo(userId);

        if (user is null)
            return NotFound();

        ViewData["IsFullscreen"] = true;

        return View(new ChatViewModel(
            user,
            messages));
    }

    [HttpPost]
    public async Task<IActionResult> Chat(string id, [FromBody] NewChatMessageViewModel viewModel)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(viewModel.Text.Trim()))
            return Ok("");

        if (!Guid.TryParse(id, out Guid userId))
            return Ok("");

        var currentUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");

        await messagesService.InsertMessage(viewModel.Text.Trim(), currentUser.Id, userId);

        var messages = await messagesService.GetChatMessages(userId, currentUser.Id, viewModel.LatestMessageTime is long time ? new DateTime(time) : null);

        return PartialView("_ChatMessages", messages);
    }

    public async Task<IActionResult> MessagesSince(string id, [FromQuery] string? latestMessageTime)
    {
        if (!Guid.TryParse(id, out Guid userId))
            return Ok("");

        var currentUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var messages = await messagesService.GetChatMessages(userId, currentUser.Id, long.TryParse(latestMessageTime, out long time) ? new DateTime(time) : null);

        return PartialView("_ChatMessages", messages);
    }
}