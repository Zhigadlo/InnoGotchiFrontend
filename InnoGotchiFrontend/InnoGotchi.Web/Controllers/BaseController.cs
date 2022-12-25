using Hanssens.Net;
using InnoGotchi.Web.BLL.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class BaseController : Controller
    {
        private IHttpClientFactory _httpClientFactory;
        protected const string _securityTokenKey = "security_token";
        private LocalStorage _storage;
        public BaseController(IHttpClientFactory httpClientFactory, LocalStorage storage)
        {
            _httpClientFactory = httpClientFactory;
            _storage = storage;
        }

        protected HttpClient GetHttpClient(string clientName)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);
            
            if (_storage.Exists(_securityTokenKey))
            {
                var securityToken = _storage.Get<SecurityToken>(_securityTokenKey);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", securityToken.AccessToken);
            }
            return httpClient;
        }
    }
}
