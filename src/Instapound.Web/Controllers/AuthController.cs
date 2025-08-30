using Instapound.Data.Entities;
using Instapound.Web.Extensions;
using Instapound.Web.Models;
using Instapound.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Instapound.Web.Controllers;

public class AuthController(UserManager<User> userManager, SignInManager<User> signInManager, ImagesService imagesService) : Controller
{
    private readonly UserManager<User> userManager = userManager;
    private readonly SignInManager<User> signInManager = signInManager;
    private readonly ImagesService imagesService = imagesService;


    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        Microsoft.AspNetCore.Identity.SignInResult result =
            await signInManager.PasswordSignInAsync(viewModel.UserName, viewModel.Password, viewModel.RememberMe, false);

        if (result.Succeeded)
            return this.RedirectToControllerAction(nameof(HomeController), nameof(HomeController.Index));

        ModelState.AddModelError("Login error", "Invalid login details");
        return View(viewModel);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        string? avatarFileName = await imagesService.SaveFormImage(viewModel.Avatar, Constants.AvatarImagesFolder);

        var user = new User
        {
            UserName = viewModel.UserName.Trim(),
            FirstName = viewModel.FirstName.Trim(),
            LastName = viewModel.LastName.Trim(),
            DateOfBirth = viewModel.DateOfBirth,
            Bio = string.IsNullOrWhiteSpace(viewModel.Bio?.Trim()) ? null : viewModel.Bio?.Trim(),
            Avatar = avatarFileName,
        };
        var result = await userManager.CreateAsync(user, viewModel.Password);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, isPersistent: true);
            return this.RedirectToControllerAction(nameof(HomeController), nameof(HomeController.Index));
        }

        foreach (IdentityError error in result.Errors)
            ModelState.AddModelError(error.Code, error.Description);

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return this.RedirectToControllerAction(nameof(HomeController), nameof(HomeController.Index));
    }
}