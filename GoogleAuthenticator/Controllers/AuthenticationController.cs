using GoogleAuthenticator.Models.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace GoogleAuthenticator.Controllers;

public class AuthenticationController(
    UserManager<IdentityUser<Guid>> userManager,
    SignInManager<IdentityUser<Guid>> signInManager)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> SignIn() => View();

    [HttpGet]
    public async Task<IActionResult> SignUp() => View();

    [HttpPost]
    public async Task<IActionResult> DefaultSignIn([FromForm] SignInModel model)
    {
        Console.WriteLine("***Default Sign In***");

        IdentityUser<Guid>? dbRecord = await userManager.FindByNameAsync(model.UserName);
        if (dbRecord is null)
        {
            return View("SignIn");
        }

        SignInResult result = await signInManager.PasswordSignInAsync(dbRecord, model.Password, false, true);
        if (result.Succeeded)
        {
            return Redirect("/");
        }
        return View("SignIn");
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginProcess()
    {
        Console.WriteLine("***External Authentication***");

        ExternalLoginInfo? loginInfo = await signInManager.GetExternalLoginInfoAsync();
        if (loginInfo is null || loginInfo.Principal is null)
        {
            return View("SignIn");
        }

        string? value = loginInfo.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            return View("SignUp");
        }

        IdentityUser<Guid>? dbRecord = await userManager.FindByNameAsync(value);
        if (dbRecord is null)
        {
            return await DefaultSignUp(new SignInModel(value, "1234567"));
        }
        return await DefaultSignIn(new SignInModel(value, "1234567"));
    }

    [HttpPost]
    public async Task<IActionResult> ExternalLogin(string provider)
    {
        AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider, Url.Action("ExternalLoginProcess", "Authentication"));
        return new ChallengeResult(provider, properties);
    }

    [HttpPost]
    public async Task<IActionResult> DefaultSignUp([FromForm] SignInModel model)
    {
        Console.WriteLine("***Default Sign Up***");

        IdentityUser<Guid>? dbRecord = await userManager.FindByNameAsync(model.UserName);
        if (dbRecord is not null)
        {
            return View("SignUp");
        }

        dbRecord = new IdentityUser<Guid>(model.UserName);

        IdentityResult createResult = await userManager.CreateAsync(dbRecord, model.Password);
        if (!createResult.Succeeded)
        {
            return View("SignUp");
        }

        var users = await userManager.Users.ToListAsync();
        
        SignInResult signInResult = await signInManager.PasswordSignInAsync(dbRecord, model.Password, false, true);
        if (signInResult.Succeeded)
        {
            return Redirect("/");
        }
        return View("SignUp");
    }

    [HttpGet]
    public async Task<IActionResult> Users() => Ok(await userManager.Users.ToListAsync());

    [HttpGet]
    public async Task<IActionResult> SignOut()
    {
        await signInManager.SignOutAsync();
        return View("SignIn");
    }
}