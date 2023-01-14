using Hanssens.Net;
using InnoGotchi.BLL.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class BaseController : Controller
    {
        private IHttpClientFactory _httpClientFactory;
        protected LocalStorage _localStorage;
        public BaseController(IHttpClientFactory httpClientFactory, LocalStorage localStorage)
        {
            _httpClientFactory = httpClientFactory;
            _localStorage = localStorage;
        }

        protected HttpClient GetHttpClient(string clientName)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);

            if (_localStorage.Exists(nameof(SecurityToken)))
            {
                string jsonToken = _localStorage.Get<string>(nameof(SecurityToken));
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
