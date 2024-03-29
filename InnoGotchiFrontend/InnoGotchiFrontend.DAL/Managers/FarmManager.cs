﻿using Hanssens.Net;
using InnoGotchi.DAL.Identity;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace InnoGotchi.DAL.Managers
{
    /// <summary>
    /// Class that have access to farm entities from server
    /// </summary>
    public class FarmManager : BaseManager
    {
        public FarmManager(IHttpClientFactory httpClientFactory,
                           LocalStorage localStorage,
                           IConfiguration configuration) : base(httpClientFactory, localStorage, configuration)
        {
        }
        /// <summary>
        /// Sends farm creation request to server
        /// </summary>
        public async Task<int> Create(int ownerId, string name)
        {
            var httpClient = GetHttpClient("Farms");

            var parameters = new Dictionary<string, string>();
            parameters["Name"] = name;
            parameters["OwnerId"] = ownerId.ToString();

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));
            if (!httpResponseMessage.IsSuccessStatusCode)
                return -1;
            
            int farmId = JsonSerializer.Deserialize<int>(await httpResponseMessage.Content.ReadAsStringAsync());
            string? jsonToken = _localStorage.Get<string>(_tokenName);
            SecurityTokenModel securityToken = JsonSerializer.Deserialize<SecurityTokenModel>(jsonToken);
            securityToken.FarmId = farmId;
            _localStorage.Remove(_tokenName);
            jsonToken = JsonSerializer.Serialize(securityToken);
            _localStorage.Store(_tokenName, jsonToken);
            return farmId;
        }
        /// <summary>
        /// Gets farm by id from server 
        /// </summary>
        public async Task<Farm?> Get(int id)
        {
            var httpClient = GetHttpClient("Farms");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + $"/{id}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
                return null;
            
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (contentStream.Length == 0)
                return null;
            Farm? farm = await JsonSerializer.DeserializeAsync<Farm>(contentStream, options);

            return farm;
        }
        /// <summary>
        /// Gets all farms from server
        /// </summary>
        public async Task<IEnumerable<Farm>?> GetAll()
        {
            var httpClient = GetHttpClient("Farms");
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
            if (contentStream.Length == 0)
                return null;
            IEnumerable<Farm>? farms = await JsonSerializer.DeserializeAsync<IEnumerable<Farm>>(contentStream, options);

            return farms;
        }
        /// <summary>
        /// Gets all farm names from server
        /// </summary>
        public async Task<IEnumerable<string>?> GetAllNames()
        {
            var httpClient = GetHttpClient("Farms");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + "/getAllNames"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
                return null;
            
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (contentStream.Length == 0)
                return null;
            IEnumerable<string>? names = await JsonSerializer.DeserializeAsync<IEnumerable<string>>(contentStream, options);

            return names;
        }
    }
}
