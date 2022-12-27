using InnoGotchi.DAL.Models;
using InnoGotchi.Web.BLL.DTO;
using InnoGotchi.Web.BLL.Services;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class FarmController : BaseController
    {
        private FarmService _farmService;
        public FarmController(IHttpClientFactory httpClientFactory,
                              FarmService farmService) : base(httpClientFactory)
        {
            _farmService = farmService;
        }

        public async Task<FarmDTO?> Get(int id)
        {
            var httpClient = await GetHttpClient("Farms");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + $"/{id}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                Farm? farm = await JsonSerializer.DeserializeAsync<Farm>(contentStream, options);

                return _farmService.GetFarmDTO(farm);
            }
            else
                return null;
        }
    }
}
