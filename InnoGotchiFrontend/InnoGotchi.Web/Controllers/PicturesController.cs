using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Services;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.Models;
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

        public async Task<IActionResult> PetConstructor()
        {
            List<PictureDTO> allPictures = (await GetAll()).ToList();
            PetConstructorViewModel vm = new PetConstructorViewModel
            {
                Bodies = allPictures.Where(p => p.Description.ToLower() == "body").ToList(),
                Eyes = allPictures.Where(p => p.Description.ToLower() == "eyes").ToList(),
                Noses = allPictures.Where(p => p.Description.ToLower() == "nose").ToList(),
                Mouths = allPictures.Where(p => p.Description.ToLower() == "mouth").ToList(),
            };
            return View(vm);
        }
        public async Task<IActionResult> AllBodyParts()
        {
            return View(await GetAll());
        }

        [HttpGet]
        public async Task<IEnumerable<PictureDTO>?> GetAll()
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

                return _service.GetAll(pictures);
            }
            else
                return null;
        }
    }
}
