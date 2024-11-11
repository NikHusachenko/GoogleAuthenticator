using GoogleAuthenticator.EntityFramework.Entities;
using GoogleAuthenticator.Models.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoogleAuthenticator.Controllers;

public class AuthenticationController : Controller
{
    [HttpGet]
    public async Task<IActionResult> SignIn() => View();

    [HttpGet]
    public async Task<IActionResult> SignUp() => View();

    [HttpPost]
    public async Task<IActionResult> DefaultSignIn([FromForm] SignInModel model)
    {
        UserEntity entity = new UserEntity()
        {
            Id = Guid.NewGuid(),
            Login = model.Login,
            Password = model.Password
        };

        ClaimsPrincipal principal =
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new("sub", entity.Id.ToString()),
                        new("name", entity.Login),
                        new("ait", DateTimeOffset.Now.Ticks.ToString())
                    ],
                    CookieAuthenticationDefaults.AuthenticationScheme));

        HttpContext.User = principal;
        return Redirect("/");
    }
    
    [HttpPost]
    public async Task<IActionResult> GoogleSignIn(string redirectUrl = "/") =>
        Challenge(new AuthenticationProperties()
        {
            RedirectUri = redirectUrl
        },
        GoogleDefaults.AuthenticationScheme);

    [HttpGet]
    public async Task<IActionResult> SignOut()
    {
        await HttpContext.SignOutAsync();
        return View("SignUp");
    }
}