using Hanssens.Net;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace InnoGotchi.DAL.Managers
{
    /// <summary>
    /// Class that have access to picture entities from server
    /// </summary>
    public class PictureManager : BaseManager
    {
        public PictureManager(IHttpClientFactory httpClientFactory,
                              LocalStorage localStorage,
                              IConfiguration configuration) : base(httpClientFactory, localStorage, configuration)
        {
        }
        /// <summary>
        /// Gets all pictures from server
        /// </summary>
        public async Task<IEnumerable<Picture>?> GetAll()
        {
            var httpClient = GetHttpClient("Pictures");

            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
                return null;
            
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            IEnumerable<Picture>? pictures = await JsonSerializer.DeserializeAsync<IEnumerable<Picture>>(contentStream, options);

            return pictures;
        }
    }
}
