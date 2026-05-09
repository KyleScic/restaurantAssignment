using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace ProgrammingForTheCloud.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            // This triggers the Google Challenge
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("Index", "Home") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}