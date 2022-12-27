using AutoMapper;
using InnoGotchi.Web.BLL.Mapper;
using InnoGotchi.Web.BLL.Services;
using InnoGotchi.Web.Extensions;
using InnoGotchi.Web.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;

string baseRoot = "https://localhost:7200/api";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(baseRoot, "Users");
builder.Services.AddHttpClient(baseRoot, "Pets");
builder.Services.AddHttpClient(baseRoot, "Farms");

var config = new MapperConfiguration(cnf => cnf.AddProfile<MapperProfile>());
builder.Services.AddTransient<IMapper>(x => new Mapper(config));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddAuthorization();

builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<FarmService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ImageService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

