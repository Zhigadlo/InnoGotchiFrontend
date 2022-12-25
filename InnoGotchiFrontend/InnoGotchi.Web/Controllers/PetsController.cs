using Hanssens.Net;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class PetsController : BaseController
    {
        private PetService _service;
        public PetsController(IHttpClientFactory httpClientFactory, 
                              PetService service,
                              LocalStorage storage) : base(httpClientFactory, storage)
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
    }
}
