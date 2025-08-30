using Instapound.Data.Entities;
using Instapound.Web.Extensions;
using Instapound.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Instapound.Web.Controllers;

[Authorize]
public class CommentsController(UserManager<User> userManager, PostsService postsService, CommentsService commentsService) : Controller
{
    private readonly UserManager<User> userManager = userManager;
    private readonly PostsService postsService = postsService;
    private readonly CommentsService commentsService = commentsService;

    [HttpPost]
    public async Task<IActionResult> Like(string id)
    {
        if (!Guid.TryParse(id, out Guid commentId))
            return NotFound();

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var model = await commentsService.LikeComment(commentId, user.Id);

        return PartialView("_LikeButtonContent", model);
    }

    [HttpPost]
    public async Task<IActionResult> Remove(string id)
    {
        if (!Guid.TryParse(id, out Guid commentId))
            return NotFound();

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var postId = await commentsService.RemoveComment(commentId, user.Id);

        return this.RedirectToControllerAction(nameof(PostsController), nameof(PostsController.Index), new { id = postId });
    }
}