using Instapound.Data.Entities;
using Instapound.Web.Models;
using Instapound.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Instapound.Web.Controllers;

[Authorize]
public class ProfileController(UserManager<User> userManager, UsersService usersService, PostsService postsService, ImagesService imagesService) : Controller
{
    private readonly UserManager<User> userManager = userManager;
    private readonly UsersService usersService = usersService;
    private readonly PostsService postsService = postsService;
    private readonly ImagesService imagesService = imagesService;


    public async Task<IActionResult> Index(string id)
    {
        if (!Guid.TryParse(id, out Guid userId))
            return NotFound();

        var currentUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var profileInfo = await usersService.GetProfileInfo(userId, currentUser.Id);

        if (profileInfo is null)
            return NotFound();

        var posts = await postsService.GetPostsByAuthorId(userId, currentUser.Id);

        return View(new ProfileViewModel(
            profileInfo,
            posts));
    }

    public async Task<IActionResult> Followers(string id)
    {
        if (!Guid.TryParse(id, out Guid userId))
            return NotFound();

        var currentUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var profileInfo = await usersService.GetProfileInfo(userId, currentUser.Id);

        if (profileInfo is null)
            return NotFound();

        var followers = await usersService.GetFollowers(userId, currentUser.Id);

        return View(new ProfileUsersViewModel(
            profileInfo,
            followers));
    }

    public async Task<IActionResult> Following(string id)
    {
        if (!Guid.TryParse(id, out Guid userId))
            return NotFound();

        var currentUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var profileInfo = await usersService.GetProfileInfo(userId, currentUser.Id);

        if (profileInfo is null)
            return NotFound();

        var follows = await usersService.GetFollows(userId, currentUser.Id);

        return View(new ProfileUsersViewModel(
            profileInfo,
            follows));
    }

    public async Task<IActionResult> Edit()
    {
        var currentUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");

        return View(new EditProfileViewModel(
            currentUser.Avatar,
            new ChangePasswordViewModel { CurrentPassword = "", ConfirmNewPassword = "", NewPassword = "" },
            new ChangeUserNameViewModel { Password = "", UserName = currentUser.UserName! },
            new ChangeProfileInfoViewModel
            {
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                DateOfBirth = currentUser.DateOfBirth,
                Bio = currentUser.Bio,
            }));
    }

    [HttpPost]
    public async Task<IActionResult> Follow(string id)
    {
        if (!Guid.TryParse(id, out Guid userId))
            return NotFound();

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var model = await usersService.FollowUser(userId, user.Id);

        return PartialView("_FollowButtonContent", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return PartialView("_ChangePasswordForm", viewModel);

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var result = await userManager.ChangePasswordAsync(user, viewModel.CurrentPassword, viewModel.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);

            return PartialView("_ChangePasswordForm", viewModel);
        }

        ModelState.Clear();

        return PartialView("_ChangePasswordForm", new ChangePasswordViewModel { CurrentPassword = "", ConfirmNewPassword = "", NewPassword = "", IsSuccess = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeUserName(ChangeUserNameViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return PartialView("_ChangeUserNameForm", viewModel);

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        var changedSuccesfully = await usersService.UpdateUserName(user, viewModel.UserName);

        if (!changedSuccesfully)
        {
            ModelState.AddModelError("AlreadyExists", "User with this username already exists");
            return PartialView("_ChangeUserNameForm", viewModel);
        }

        ModelState.Clear();

        var updatedUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");

        return PartialView("_ChangeUserNameForm", new ChangeUserNameViewModel { Password = "", UserName = updatedUser.UserName!, IsSuccess = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeProfileInfo(ChangeProfileInfoViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return PartialView("_ChangeProfileInfoForm", viewModel);

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");

        user.FirstName = viewModel.FirstName;
        user.LastName = viewModel.LastName;
        user.DateOfBirth = viewModel.DateOfBirth;
        user.Bio = viewModel.Bio;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);

            return PartialView("_ChangeProfileInfoForm", viewModel);
        }

        var updatedUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");

        ModelState.Clear();

        return PartialView(
            "_ChangeProfileInfoForm",
            new ChangeProfileInfoViewModel
            {
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                DateOfBirth = updatedUser.DateOfBirth,
                Bio = updatedUser.Bio,
                IsSuccess = true
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeAvatar(UploadFileViewModel viewModel)
    {
        if (!ModelState.IsValid || viewModel.File is null)
            return RedirectToAction(nameof(Edit));

        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");
        string? avatarFileName = await imagesService.SaveFormImage(viewModel.File, Constants.AvatarImagesFolder);

        var oldAvatar = user.Avatar;
        user.Avatar = avatarFileName;
        await userManager.UpdateAsync(user);

        DeleteAvatarFile(oldAvatar);

        return RedirectToAction(nameof(Edit));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAvatar()
    {
        var user = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User cannot be null");

        var oldAvatar = user.Avatar;
        user.Avatar = null;
        await userManager.UpdateAsync(user);

        DeleteAvatarFile(oldAvatar);

        return RedirectToAction(nameof(Edit));
    }

    private void DeleteAvatarFile(string? avatar)
    {
        if (avatar is null)
            return;

        var path = $"{Constants.AvatarImagesFolder}{avatar}";

        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);
    }
}