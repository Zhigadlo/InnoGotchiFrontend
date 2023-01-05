using InnoGotchi.BLL.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace InnoGotchi.Web.Middleware
{
    public class JwtTokenCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtTokenCheckMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            SecurityToken? securityToken = GetSecurityToken(context);

            if (securityToken != null)
            {
                if (securityToken.ExpireAt < DateTime.UtcNow)
                {
                    context.Response.Cookies.Delete("security_token");
                    await context.SignOutAsync();
                }

                if (!context.User.Identity.IsAuthenticated && securityToken.ExpireAt > DateTime.UtcNow)
                {
                    await SignIn(securityToken, context);
                }
            }
            else
            {
                context.Response.Cookies.Delete("security_token");
                await context.SignOutAsync();
            }
            await _next.Invoke(context);
        }

        private SecurityToken? GetSecurityToken(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey("security_token"))
            {
                string? jsonToken = context.Request.Cookies["security_token"];
                SecurityToken? securityToken = JsonSerializer.Deserialize<SecurityToken>(jsonToken);
                return securityToken;
            }
            return null;
        }

        private async Task SignIn(SecurityToken securityToken, HttpContext context)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, securityToken.Email),
                new Claim(ClaimTypes.Name, securityToken.UserName),
                new Claim("access_token", securityToken.AccessToken),
                new Claim("expiredAt", securityToken.ExpireAt.ToString()),
                new Claim("user_id", securityToken.UserId.ToString()),
                new Claim("farm_id", securityToken.FarmId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            await context.SignInAsync(claimsPrincipal);
        }
    }
}
