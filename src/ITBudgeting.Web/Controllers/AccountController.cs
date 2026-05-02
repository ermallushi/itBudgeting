using System.Security.Claims;
using ITBudgeting.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.Web.Controllers;

public class AccountController : Controller
{
    private static readonly Dictionary<string, (string Password, string Role)> Users = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin"]             = ("Admin1!",    "Admin"),
        ["manager"]           = ("Manager1!",  "Manager"),
        ["financecontroller"] = ("Finance1!",  "FinanceController"),
        ["user"]              = ("User1!",     "BudgetUser"),
    };

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (!Users.TryGetValue(vm.Username, out var creds) || creds.Password != vm.Password)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(vm);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, vm.Username),
            new(ClaimTypes.Role, creds.Role),
        };
        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = false });

        return LocalRedirect(vm.ReturnUrl ?? "/");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();
}
