using GoogleAuthenticator.EntityFramework;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

const string AuthenticationOptionsNotFound = "Authentication options not found";
const string GoogleClientId = "Authentication:Google:ClientId";
const string GoogleClientSecret = "Authentication:Google:ClientSecret";

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services.AddControllersWithViews();

services.AddDbContext<MemoryContext>(options => options.UseInMemoryDatabase("memory"));

services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = configuration.GetSection(GoogleClientId)?.Value ?? throw new InvalidOperationException(AuthenticationOptionsNotFound);
    options.ClientSecret = configuration.GetSection(GoogleClientSecret)?.Value ?? throw new InvalidOperationException(AuthenticationOptionsNotFound);
    options.CallbackPath = "/authentication/GoogleSignInProccess";
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/authentication/signup";
    options.LogoutPath = "/authentication/signout";
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();