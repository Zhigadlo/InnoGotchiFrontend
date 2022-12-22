using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace InnoGotchi.Web.Controllers
{
    public class BaseController : Controller
    {
        private IHttpClientFactory _httpClientFactory;
        protected const string _tokenKey = "access_token";
        public BaseController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected HttpClient GetHttpClient(string clientName)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);

            string? token = HttpContext.Request.Cookies[_tokenKey];

            if(token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return httpClient;
        }
    }
}
