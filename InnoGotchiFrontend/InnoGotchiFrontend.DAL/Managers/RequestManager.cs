using Hanssens.Net;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.DAL.Managers
{
    public class RequestManager : BaseManager
    {
        public RequestManager(IHttpClientFactory httpClientFactory,
                              LocalStorage localStorage,
                              IConfiguration configuration) : base(httpClientFactory, localStorage, configuration)
        {
        }

        public async Task<bool> Create(int ownerId, int receiverId)
        {
            var httpClient = GetHttpClient("Requests");

            var parameters = new Dictionary<string, string>();
            parameters["IsConfirm"] = false.ToString();
            parameters["RequestOwnerId"] = ownerId.ToString();
            parameters["RequestReceipientId"] = receiverId.ToString();

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            else
                return false;
        }

        public async Task<bool> Confirm(int requestId)
        {
            var httpClient = GetHttpClient("Requests");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Put,
                httpClient.BaseAddress + $"/confirm/{requestId}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            else
                return false;
        }

        public async Task<bool> Delete(int requestId)
        {
            var httpClient = GetHttpClient("Requests");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Delete,
                httpClient.BaseAddress + $"/{requestId}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            else
                return false;
        }
    }
}
