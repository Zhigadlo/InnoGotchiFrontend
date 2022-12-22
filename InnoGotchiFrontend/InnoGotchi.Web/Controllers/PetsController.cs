using InnoGotchi.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class PetsController : BaseController
    {
        public PetsController(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            
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
           
                return View(pet);
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
                if (pets == null)
                    pets = new List<Pet>();
                return View(pets);
            }
            else
                return BadRequest();
        }
    }
}
