using InnoGotchi.DAL.Models;
using InnoGotchi.Web.BLL.DTO;
using InnoGotchi.Web.BLL.Services;
using InnoGotchi.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class RequestsController : BaseController
    {
        private RequestService _requestService;
        public RequestsController(IHttpClientFactory httpClientFactory,
                                  RequestService requestService) : base(httpClientFactory)
        {
            _requestService = requestService;   
        }

        public async Task<IActionResult> UserRequests()
        {
            UserRequestsViewModel requests = new UserRequestsViewModel 
            { 
                SentRequests = await GetSentRequests(),
                ReceivedRequests = await GetReceivedRequests()
            };

            return View(requests);
        }


        public async Task<IEnumerable<ColoborationRequestDTO>?> GetSentRequests()
        {
            var httpClient = GetHttpClient("Requests");

            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + $"/sentRequests/{HttpContext.User.FindFirstValue("user_id")}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                if (contentStream.Length == 0)
                    return null;
                IEnumerable<ColoborationRequest>? requests = await JsonSerializer.DeserializeAsync<IEnumerable<ColoborationRequest>>(contentStream, options);

                return _requestService.GetColoborationRequestsDTO(requests);
            }
            else
                return null;
        }

        public async Task<IEnumerable<ColoborationRequestDTO>?> GetReceivedRequests()
        {
            var httpClient = GetHttpClient("Requests");

            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + $"/receivedRequests/{HttpContext.User.FindFirstValue("user_id")}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                if (contentStream.Length == 0)
                    return null;
                IEnumerable<ColoborationRequest>? requests = await JsonSerializer.DeserializeAsync<IEnumerable<ColoborationRequest>>(contentStream, options);

                return _requestService.GetColoborationRequestsDTO(requests);
            }
            else
                return null;
        }

    }
}
