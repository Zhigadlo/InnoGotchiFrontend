using Hanssens.Net;
using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Identity;
using InnoGotchi.BLL.Services;
using InnoGotchi.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class PetsController : BaseController
    {
        private PetService _service;
        public PetsController(IHttpClientFactory httpClientFactory,
                              PetService service,
                              LocalStorage localStorage) : base(httpClientFactory, localStorage)
        {
            _service = service;
        }

        public async Task<IActionResult> Get(int id)
        {
            var httpClient = GetHttpClient("Pets");
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
                Pet? pet = await JsonSerializer.DeserializeAsync<Pet>(contentStream, options);

                return View(_service.Get(pet));
            }
            else
                return BadRequest();
        }

        [HttpGet]
        public async Task<IEnumerable<PetDTO>?> GetAllPets()
        {
            var httpClient = GetHttpClient("Pets");

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
                IEnumerable<Pet>? pets = await JsonSerializer.DeserializeAsync<IEnumerable<Pet>>(contentStream, options);

                return _service.GetAll(pets);
            }
            else
                return null;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string appearance)
        {
            var httpClient = GetHttpClient("Pets");

            var parameters = new Dictionary<string, string>();
            parameters["Name"] = name;
            parameters["Appearance"] = appearance;
            parameters["FarmId"] = HttpContext.User.FindFirstValue(nameof(SecurityToken.FarmId));

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("UserFarm", "Farms", new { id = HttpContext.User.FindFirstValue(nameof(SecurityToken.UserId)) });
            }
            else
                return BadRequest();
        }

        public async Task<IActionResult> Feed(int id)
        {
            var httpClient = GetHttpClient("Pets");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Put,
                httpClient.BaseAddress + $"/feed/{id}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("GetUserFarm", "Farms");
            }
            else
                return BadRequest();
        }

        public async Task<IActionResult> Drink(int id)
        {
            var httpClient = GetHttpClient("Pets");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Put,
                httpClient.BaseAddress + $"/drink/{id}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("GetUserFarm", "Farms");
            }
            else
                return BadRequest();
        }
    }
}
