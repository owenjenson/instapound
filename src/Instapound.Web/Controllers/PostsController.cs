using Instapound.Data.Entities;
using Instapound.Web.Extensions;
using Instapound.Web.Models;
using Instapound.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Instapound.Web.Controllers;

[Authorize]
public class PostsController(UserManager<User> userManager, PostsService postsService, CommentsService commentsService, ImagesService imagesService) : Controller
{
    private readonly UserManager<User> userManager = userManager;
    private readonly PostsService postsService = postsService;
    private readonly CommentsService commentsService = commentsService;
    private readonly ImagesService imagesService = imagesService;


    public async Task<IActionResult> Index(string id)
    {
        if (!Guid.TryParse(id, out Guid postId))
            return NotFound();

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var post = await postsService.GetPost(postId, user.Id);
        var comments = await commentsService.GetPostComments(postId, user.Id);

        if (post is null)
            return NotFound();

        return View(new PostWithCommentsViewModel(post, comments));
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewPostViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        var imageFileName = await imagesService.SaveFormImage(viewModel.Image, Constants.PostImagesFolder);

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var postId = await postsService.InsertPost(
            viewModel.Text,
            imageFileName,
            user.Id);

        return RedirectToAction(nameof(Index), new { id = postId.ToString() });
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (!Guid.TryParse(id, out Guid postId))
            return NotFound();

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var post = await postsService.GetPostToEdit(postId, user.Id);

        if (post is null)
            return NotFound();

        return View(post);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, EditPostViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        if (!Guid.TryParse(id, out Guid postId))
            return NotFound();

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var imageFileName = await imagesService.SaveFormImage(viewModel.NewImage, Constants.PostImagesFolder);
        await postsService.UpdatePost(postId, user.Id, viewModel.Text, imageFileName, viewModel.ChangedImage);

        return RedirectToAction(nameof(Index), new { id = postId.ToString() });
    }

    [HttpPost]
    public async Task<IActionResult> Remove(string id)
    {
        if (!Guid.TryParse(id, out Guid postId))
            return NotFound();

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        await postsService.RemovePost(postId, user.Id);

        return this.RedirectToControllerAction(nameof(ProfileController), nameof(ProfileController.Index), new { id = user.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Like(string id)
    {
        if (!Guid.TryParse(id, out Guid postId))
            return NotFound();

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var model = await postsService.LikePost(postId, user.Id);

        return PartialView("_LikeButtonContent", model);
    }

    [HttpPost]
    public async Task<IActionResult> Comment(NewCommentViewModel viewModel)
    {
        if (!Guid.TryParse(viewModel.PostId, out Guid postId))
            return NotFound();

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        await postsService.CommentPost(postId, user.Id, viewModel.Text);

        return RedirectToAction(nameof(Index), new { id = viewModel.PostId });
    }
}