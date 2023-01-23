using Hanssens.Net;
using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Identity;
using InnoGotchi.BLL.Models;
using InnoGotchi.BLL.Services;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.Models;
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

        public async Task<IActionResult> PetView(int id)
        {
            var pet = await Get(id);
            return View(pet);
        }

        public async Task<PetDTO?> Get(int id)
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

                return _service.Get(pet);
            }
            else
                return null;
        }

        public async Task<PaginatedList<PetDTO>> GetPage(int page, string sortType)
        {
            var httpClient = GetHttpClient("Pets");

            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + $"/{page}&{sortType}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                PaginatedList<Pet>? pets = await JsonSerializer.DeserializeAsync<PaginatedList<Pet>>(contentStream, options);

                return _service.GetPage(pets);
            }
            else
                return null;
        }

        public async Task<IActionResult> AllPetsView(int page = 1, string sortType = "happiness_asc")
        {
            var pets = await GetPage(page, sortType);
            if (pets == null)
                return RedirectToAction("Login", "Users");
            var vm = new AllPetsViewModel
            {
                PaginatedList = pets,
                SortModel = new PetSortModel(sortType)
            };
            return View(vm);
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
                return RedirectToAction("UserFarm", "Farms", new { id = HttpContext.User.FindFirstValue(nameof(SecurityToken.FarmId)) });
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
                return RedirectToAction("GetCurrentUserFarm", "Farms");
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
                return RedirectToAction("GetCurrentUserFarm", "Farms");
            }
            else
                return BadRequest();
        }
    }
}
