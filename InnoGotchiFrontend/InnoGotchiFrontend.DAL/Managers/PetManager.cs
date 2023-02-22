using Hanssens.Net;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace InnoGotchi.DAL.Managers
{
    /// <summary>
    /// Class that have access to pet entities from server
    /// </summary>
    public class PetManager : BaseManager
    {
        public PetManager(IHttpClientFactory httpClientFactory,
                          LocalStorage localStorage,
                          IConfiguration configuration) : base(httpClientFactory, localStorage, configuration)
        {
        }
        /// <summary>
        /// Gets pet by id from server
        /// </summary>
        public async Task<Pet?> Get(int id)
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

                return pet;
            }
            else
                return null;
        }
        /// <summary>
        /// Gets pets on page with sorting and filatration froma server
        /// </summary>
        public async Task<PaginatedList<List<Pet>>?> GetPage(int page, string sortType, PetFilterModel filterModel)
        {
            var httpClient = GetHttpClient("Pets");
            var path = new StringBuilder($"/getPage/?page={page}&sortType={sortType}");

            if (filterModel.Age != 0 && filterModel.GameYear != 0)
            {
                path.Append($"&Age={filterModel.Age}");
                path.Append($"&GameYear={filterModel.GameYear}");
            }

            if (filterModel.HungerLavel != -1)
            {
                path.Append($"&HungerLavel={filterModel.HungerLavel}");
                path.Append($"&FeedingPeriod={filterModel.FeedingPeriod}");
                path.Append($"&IsLastHungerStage={filterModel.IsLastHungerStage}");
            }

            if (filterModel.ThirstyLavel != -1)
            {
                path.Append($"&ThirstyLavel={filterModel.ThirstyLavel}");
                path.Append($"&DrinkingPeriod={filterModel.DrinkingPeriod}");
                path.Append($"&IsLastThirstyStage={filterModel.IsLastThirstyStage}");
            }

            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + path.ToString()
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                PaginatedList<List<Pet>>? pets = await JsonSerializer.DeserializeAsync<PaginatedList<List<Pet>>>(contentStream, options);
                if (pets == null)
                    pets = new PaginatedList<List<Pet>>();

                return pets;
            }
            else
                return null;
        }
        /// <summary>
        /// Gets all pets froma server
        /// </summary>
        public async Task<IEnumerable<Pet>?> GetAll()
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

                return pets;
            }
            else
                return null;
        }
        /// <summary>
        /// Gets all pet names from server
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>?> GetAllNames()
        {
            var httpClient = GetHttpClient("Pets");

            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + "/getAllNames"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                IEnumerable<string>? names = await JsonSerializer.DeserializeAsync<IEnumerable<string>>(contentStream, options);

                return names;
            }
            else
                return null;
        }
        /// <summary>
        /// Sets dead status to pet by id on server 
        /// </summary>
        public async Task<bool> Death(int id, long deathTime)
        {
            var httpClient = GetHttpClient("Pets");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Put,
                httpClient.BaseAddress + $"/death/{id}&{deathTime}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Gives a drink to pet by id
        /// </summary>
        public async Task<bool> Drink(int id)
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
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Feeds pet by id
        /// </summary>
        public async Task<bool> Feed(int id)
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
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Sends request to create new pet 
        /// </summary>
        public async Task<int> Create(string name, string appearance, int farmId)
        {
            var httpClient = GetHttpClient("Pets");

            var parameters = new Dictionary<string, string>();
            parameters["Name"] = name;
            parameters["Appearance"] = appearance;
            parameters["FarmId"] = farmId.ToString();

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                int petId = await JsonSerializer.DeserializeAsync<int>(contentStream, options);

                return petId;
            }
            else
                return -1;
        }
    }
}
