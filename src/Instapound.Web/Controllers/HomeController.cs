using Instapound.Data.Entities;
using Instapound.Web.Extensions;
using Instapound.Web.Models;
using Instapound.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Instapound.Web.Controllers;

[Authorize]
public class HomeController(UserManager<User> userManager, PostsService postsService) : Controller
{
    private const int PostsPerPage = 10;

    private readonly UserManager<User> userManager = userManager;
    private readonly PostsService postsService = postsService;


    public async Task<IActionResult> Index([FromQuery] int? page = null)
    {
        var user = await userManager.GetUserAsync(User);

        if (user is null)
            return this.RedirectToControllerAction(nameof(AuthController), nameof(AuthController.Index));

        var currentPage = page ?? 0;
        var posts = (await GetPostsToPage(user.Id, currentPage))
            .Take((currentPage + 1) * PostsPerPage)
            .ToList();

        return View(posts);
    }

    /// <summary>
    /// Endpoint for infinite loading of posts.
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<IActionResult> NextPosts(int page)
    {
        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var posts = (await GetPostsToPage(user.Id, page))
            .Skip(page * PostsPerPage)
            .Take(PostsPerPage)
            .ToList();

        return PartialView(posts);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<IEnumerable<PostViewModel>> GetPostsToPage(Guid userId, int page)
    {
        var itemsCount = (page + 1) * PostsPerPage;
        var posts = await postsService.GetAllPosts(userId);

        // TODO: Move the pagination to the database level
        return Enumerable
            .Range(0, (int)Math.Ceiling((double)itemsCount / posts.Count))
            .SelectMany((i) => posts);
    } 
}