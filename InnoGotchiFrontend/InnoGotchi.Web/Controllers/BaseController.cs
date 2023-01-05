using InnoGotchi.BLL.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class BaseController : Controller
    {
        private IHttpClientFactory _httpClientFactory;
        protected const string _securityTokenKey = "security_token";
        public BaseController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected HttpClient GetHttpClient(string clientName)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);

            if (HttpContext.Request.Cookies.ContainsKey("security_token"))
            {
                string jsonToken = HttpContext.Request.Cookies["security_token"];
                SecurityToken? securityToken = JsonSerializer.Deserialize<SecurityToken>(jsonToken);
                if (securityToken != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", securityToken.AccessToken);
                }
            }
            return httpClient;
        }
    }
}
