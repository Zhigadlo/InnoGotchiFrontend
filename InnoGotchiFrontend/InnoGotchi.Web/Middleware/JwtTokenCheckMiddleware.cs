﻿using Hanssens.Net;
using InnoGotchi.BLL.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace InnoGotchi.Web.Middleware
{
    /// <summary>
    /// This midlleware monitors the relevance of the jwt token
    /// </summary>
    public class JwtTokenCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtTokenCheckMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context, LocalStorage localStorage, IConfiguration configuration)
        {
            string tokenName = configuration.GetSection("TokenLocalStoragename").Value;
            SecurityToken? securityToken = GetSecurityToken(localStorage, tokenName);

            if (securityToken != null)
            {
                if (securityToken.ExpireAt < DateTime.UtcNow)
                {
                    localStorage.Remove(tokenName);
                    await context.SignOutAsync();
                }

                if (!context.User.Identity.IsAuthenticated && securityToken.ExpireAt > DateTime.UtcNow)
                {
                    await SignIn(securityToken, context);
                }
            }
            else
            {
                await context.SignOutAsync();
            }
            await _next.Invoke(context);
        }

        private SecurityToken? GetSecurityToken(LocalStorage localStorage, string tokenName)
        {
            if (localStorage.Exists(tokenName))
            {
                string? jsonToken = localStorage.Get<string>(tokenName);
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
                new Claim(nameof(SecurityToken.AccessToken), securityToken.AccessToken),
                new Claim(nameof(SecurityToken.ExpireAt), securityToken.ExpireAt.ToString()),
                new Claim(nameof(SecurityToken.UserId), securityToken.UserId.ToString()),
                new Claim(nameof(SecurityToken.FarmId), securityToken.FarmId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            await context.SignInAsync(claimsPrincipal);
        }
    }
}
