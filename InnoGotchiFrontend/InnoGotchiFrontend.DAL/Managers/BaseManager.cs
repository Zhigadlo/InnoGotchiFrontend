using Hanssens.Net;
using InnoGotchi.DAL.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InnoGotchi.DAL.Managers
{
    public class BaseManager
    {
        protected IHttpClientFactory _httpClientFactory;
        protected LocalStorage _localStorage;
        protected readonly string _tokenName;
        public BaseManager(IHttpClientFactory httpClientFactory,
                           LocalStorage localStorage,
                           IConfiguration configuration)
        {
            _tokenName = configuration.GetSection("TokenLocalStoragename").Value;
            _httpClientFactory = httpClientFactory;
            _localStorage = localStorage;
        }

        protected HttpClient GetHttpClient(string clientName)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);

            if (_localStorage.Exists(_tokenName))
            {
                string jsonToken = _localStorage.Get<string>(_tokenName);
                SecurityTokenModel? securityToken = JsonSerializer.Deserialize<SecurityTokenModel>(jsonToken);
                if (securityToken != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", securityToken.AccessToken);
                }
            }
            return httpClient;
        }
    }
}

