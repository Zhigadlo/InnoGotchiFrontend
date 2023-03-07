using AutoMapper;
using Hanssens.Net;
using InnoGotchi.BLL.Mapper;
using InnoGotchi.BLL.Services;
using InnoGotchi.Web.Extensions;
using InnoGotchi.Web.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

string baseRoot = builder?.Configuration.GetSection("BaseAddress")?.Value;

builder.Services.AddSession();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(baseRoot, "Users");
builder.Services.AddHttpClient(baseRoot, "Pets");
builder.Services.AddHttpClient(baseRoot, "Farms");
builder.Services.AddHttpClient(baseRoot, "Requests");
builder.Services.AddHttpClient(baseRoot, "Pictures");

var config = new MapperConfiguration(cnf => cnf.AddProfile<MapperProfile>());
builder.Services.AddTransient<IMapper>(x => new Mapper(config));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddAuthorization();

builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<FarmService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<LocalStorage>();
builder.Services.AddTransient<PetInfoService>();

var app = builder.Build();
app.UseSession();
app.UseMiddleware<JwtTokenCheckMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHttpLogging();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();

