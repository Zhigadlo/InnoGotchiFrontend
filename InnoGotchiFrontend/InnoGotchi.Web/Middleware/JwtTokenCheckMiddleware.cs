using Hanssens.Net;
using InnoGotchi.Web.BLL.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace InnoGotchi.Web.Middleware
{
    public class JwtTokenCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtTokenCheckMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context, LocalStorage localStorage)
        {
            if (localStorage.Exists("security_token") && !context.User.Identity.IsAuthenticated)
            {
                SecurityToken securityToken = localStorage.Get<SecurityToken>("security_token");
                await SignIn(securityToken, context);
            }
            await _next.Invoke(context);
        }

        private async Task SignIn(SecurityToken securityToken, HttpContext context)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, securityToken.Email),
                new Claim(ClaimTypes.Name, securityToken.UserName),
                new Claim("access_token", securityToken.AccessToken),
                new Claim("expiredAt", securityToken.ExpireAt.ToString()),
                new Claim("user_id", securityToken.UserId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            await context.SignInAsync(claimsPrincipal);
        }
    }
}
