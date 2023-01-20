using Hanssens.Net;
using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Identity;
using InnoGotchi.BLL.Services;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class FarmsController : BaseController
    {
        private FarmService _farmService;
        public FarmsController(IHttpClientFactory httpClientFactory,
                              FarmService farmService,
                              LocalStorage localStorage) : base(httpClientFactory, localStorage)
        {
            _farmService = farmService;
        }

        public async Task<IActionResult> UserFarm(int id)
        {
            string authorizedUserId = HttpContext.User.FindFirstValue(nameof(SecurityToken.UserId));
            if(authorizedUserId == null)
                return RedirectToAction("Login", "Users");
            
            if (id == -1)
                return View("UserFarm", new UserFarmModel { FarmNames = await GetAllFarmNames(), Farm = new FarmDTO { Id = id }, AuthorizedUserId = int.Parse(authorizedUserId) } );
            FarmDTO? farm = await Get(id);
            if (farm == null)
                return RedirectToAction("Login", "Users");
            return View("UserFarm", new UserFarmModel { FarmNames = await GetAllFarmNames(), Farm = farm, AuthorizedUserId = int.Parse(authorizedUserId) });
        }

        public async Task<IActionResult> CreateFarm(string name)
        {
            var httpClient = GetHttpClient("Farms");

            var parameters = new Dictionary<string, string>();
            parameters["Name"] = name;
            parameters["OwnerId"] = HttpContext.User.FindFirstValue(nameof(SecurityToken.UserId));

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                int farmId = JsonSerializer.Deserialize<int>(await httpResponseMessage.Content.ReadAsStringAsync());
                string? jsonToken = _localStorage.Get<string>(nameof(SecurityToken));
                SecurityToken securityToken = JsonSerializer.Deserialize<SecurityToken>(jsonToken);
                securityToken.FarmId = farmId;
                _localStorage.Remove(nameof(SecurityToken));
                jsonToken = JsonSerializer.Serialize(securityToken);
                _localStorage.Store(nameof(SecurityToken), jsonToken);
                return RedirectToAction("GetCurrentUserFarm");
            }
            else
                return BadRequest();
        }

        public async Task<IActionResult> GetCurrentUserFarm()
        {
            string farmId = HttpContext.User.FindFirstValue(nameof(SecurityToken.FarmId));
            if (farmId == null)
                return RedirectToAction("Login", "Users");
            //return RedirectToAction("UserFarm", new { id = int.Parse(farmId) });
            return await UserFarm(int.Parse(farmId));
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

        public async Task<IEnumerable<string>> GetAllFarmNames()
        {
            var httpClient = GetHttpClient("Farms");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress
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
                IEnumerable<Farm>? farm = await JsonSerializer.DeserializeAsync<IEnumerable<Farm>>(contentStream, options);

                return _farmService.GetAllFarmsNames(farm);
            }
            else
                return null;
        }
    }
}
