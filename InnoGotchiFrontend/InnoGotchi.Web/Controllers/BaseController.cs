using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.Web.Controllers
{
    public class BaseController : Controller
    {
        protected IHttpClientFactory _httpClientFactory;
        public BaseController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
    }
}
