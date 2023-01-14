using Hanssens.Net;
using InnoGotchi.BLL.Identity;
using InnoGotchi.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InnoGotchi.Web.Controllers
{
    public class RequestsController : BaseController
    {
        private RequestService _requestService;
        public RequestsController(IHttpClientFactory httpClientFactory,
                                  LocalStorage localStorage,
                                  RequestService requestService) : base(httpClientFactory, localStorage)
        {
            _requestService = requestService;
        }

        public async Task<IActionResult> Create(int receiverId)
        {
            var httpClient = GetHttpClient("Requests");

            var parameters = new Dictionary<string, string>();
            parameters["IsConfirm"] = false.ToString();
            parameters["RequestOwnerId"] = HttpContext.User.FindFirstValue(nameof(SecurityToken.UserId));
            parameters["RequestReceipientId"] = receiverId.ToString();

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("UserRequests");
            }
            else
                return BadRequest();
        }
    }
}
