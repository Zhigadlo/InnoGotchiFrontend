using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace InnoGotchi.Web.Controllers
{
    public class BaseController : Controller
    {
        private IHttpClientFactory _httpClientFactory;
        protected string? _token = null;
        protected const string _tokenKey = "access_token";
        public BaseController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected HttpClient GetHttpClient(string clientName)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);

            if (_token == null)
                _token = HttpContext.Request.Cookies[_tokenKey];

            if(_token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }
            return httpClient;
        }
    }
}
