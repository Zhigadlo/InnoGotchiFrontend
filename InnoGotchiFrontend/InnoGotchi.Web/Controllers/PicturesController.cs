using InnoGotchi.BLL.Services;
using InnoGotchi.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class PicturesController : BaseController
    {
        private ImageService _service;
        public PicturesController(IHttpClientFactory httpClientFactory,
                                  ImageService imageService) : base(httpClientFactory)
        {
            _service = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var httpClient = GetHttpClient("Pictures");

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
                IEnumerable<Picture>? pictures = await JsonSerializer.DeserializeAsync<IEnumerable<Picture>>(contentStream, options);

                return View(_service.GetAll(pictures));
            }
            else
                return BadRequest();
        }
    }
}
