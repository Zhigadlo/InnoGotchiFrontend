using InnoGotchi.Web.Extensions;
using InnoGotchi.Web.Middleware;
using InnoGotchi.Web.Models;

string baseRoot = "https://localhost:7200/api";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClientForPet(baseRoot);
builder.Services.AddHttpClientForUser(baseRoot);
builder.Services.AddScoped<AuthorizedUserModel>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHttpLogging();
app.UseRouting();

app.UseMiddleware<JwtTokenCheckMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

