using AutoMapper;
using Hanssens.Net;
using InnoGotchi.Web.BLL.Identity;
using InnoGotchi.Web.BLL.Mapper;
using InnoGotchi.Web.BLL.Services;
using InnoGotchi.Web.Extensions;
using InnoGotchi.Web.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;

string baseRoot = "https://localhost:7200/api";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClientForPet(baseRoot);
builder.Services.AddHttpClientForUser(baseRoot);

var config = new MapperConfiguration(cnf => cnf.AddProfile<MapperProfile>());
builder.Services.AddTransient<IMapper>(x => new Mapper(config));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddAuthorization();

builder.Services.AddSingleton<LocalStorage>();

builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<UserService>();

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

