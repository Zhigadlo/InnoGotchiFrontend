using InnoGotchi.Web.Models;

namespace InnoGotchi.Web.Middleware
{
    public class JwtTokenCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtTokenCheckMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context, AuthorizedUserModel userModel)
        {
            var token = context.Request.Cookies["access_token"];
            userModel.AccessToken = token;
            await _next.Invoke(context);
        }
    }
}
