using InnoGotchi.DAL.Models;
using InnoGotchi.Web.BLL.DTO;
using InnoGotchi.Web.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class PetsController : BaseController
    {
        private PetService _service;
        public PetsController(IHttpClientFactory httpClientFactory,
                              PetService service) : base(httpClientFactory)
        {
            _service = service;
        }

        public IActionResult PetConstructor()
        {
            return View();
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
        public async Task<IActionResult> GetAll()
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

                return View(_service.GetAll(pets));
            }
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, Appearance? appearance)
        {
            var httpClient = GetHttpClient("Pets");

            var parameters = new Dictionary<string, string>();
            parameters["Name"] = name;
            parameters["Appearance.EyesURL"] = appearance.EyesURL;
            parameters["Appearance.NoseURL"] = appearance.NoseURL;
            parameters["Appearance.MouthURL"] = appearance.MouthURL;
            parameters["Appearance.BodyURL"] = appearance.BodyURL;
            parameters["FarmId"] = HttpContext.User.FindFirstValue("farm_id");

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("UserFarm", "Farms", new { id = HttpContext.User.FindFirstValue("user_id") });
            }
            else
                return BadRequest();
        }
    }
}
