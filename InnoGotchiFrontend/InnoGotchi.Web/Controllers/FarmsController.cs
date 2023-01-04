using InnoGotchi.DAL.Models;
using InnoGotchi.Web.BLL.DTO;
using InnoGotchi.Web.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class FarmsController : BaseController
    {
        private FarmService _farmService;
        public FarmsController(IHttpClientFactory httpClientFactory,
                              FarmService farmService) : base(httpClientFactory)
        {
            _farmService = farmService;
        }

        public async Task<IActionResult> UserFarm(int id)
        {
            FarmDTO? farm = await Get(id);
            return View(farm);
        }

        public async Task<IActionResult> CreateFarm(string name)
        {
            var httpClient = GetHttpClient("Farms");

            var parameters = new Dictionary<string, string>();
            parameters["Name"] = name;
            parameters["OwnerId"] = HttpContext.User.FindFirstValue("user_id");

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                int farmId = JsonSerializer.Deserialize<int>(await httpResponseMessage.Content.ReadAsStringAsync());
                
                return RedirectToAction("UserFarm", new { id = farmId });
            }
            else
                return BadRequest();
        }

        public IActionResult GetUserFarm()
        {
            int farmId = int.Parse(HttpContext.User.FindFirstValue("farm_id"));
            return RedirectToAction("UserFarm", new { id = farmId } );
        }

        public async Task<FarmDTO?> Get(int id)
        {
            var httpClient = GetHttpClient("Farms");
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
                if (contentStream.Length == 0)
                    return null;
                Farm? farm = await JsonSerializer.DeserializeAsync<Farm>(contentStream, options);

                return _farmService.GetFarmDTO(farm);
            }
            else
                return null;
        }
    }
}
