using Microsoft.AspNetCore.Mvc;

namespace Instapound.Web.Extensions;

public static class ControllerExtensions
{
    public static IActionResult RedirectToControllerAction(this Controller controller, string controllerClass, string action, object? routeValues = null) =>
        controller.RedirectToAction(action, controllerClass.Replace("Controller", ""), routeValues);
}
