using Hanssens.Net;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.BLL.DTO;
using InnoGotchi.Web.BLL.Identity;
using InnoGotchi.Web.BLL.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class UsersController : BaseController
    {
        private UserService _service;
        private LocalStorage _storage;
        public UsersController(IHttpClientFactory httpClientFactory,
                               UserService service,
                               LocalStorage storage) : base(httpClientFactory, storage)
        {
            _service = service;
            _storage = storage;
        }

        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            _storage.Remove(_securityTokenKey);
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Authenticate(string email, string password)
        {
            string? token = await Token(email, password);
            if (token != null)
            {
                var user = await GetUser(email, password);
                var securityToken = new SecurityToken
                {
                    AccessToken = token,
                    Email = email,
                    UserName = user.FirstName + " " + user.LastName,
                    UserId = user.Id,
                    ExpireAt = DateTime.UtcNow.AddHours(6)
                };
                _storage.Store(_securityTokenKey, securityToken);
                return RedirectToAction("Index", "Home");
            }
            else
                return BadRequest();
        }
        private async Task<string?> Token(string email, string password)
        {
            var httpClient = GetHttpClient("Users");
            var parameters = new Dictionary<string, string>();
            parameters["email"] = email;
            parameters["password"] = password;

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress + "/token", new FormUrlEncodedContent(parameters));

            string? token = null;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                token = (await httpResponseMessage.Content.ReadAsStringAsync()).Replace("\"", String.Empty);
            }
            return token;
        }
        private async Task<UserDTO?> GetUser(string email, string password)
        {
            var httpClient = GetHttpClient("Users");

            var parameters = new Dictionary<string, string>();

            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + $"/{email}&{password}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var user = await JsonSerializer.DeserializeAsync<User>(contentStream, options);
                return _service.Get(user);
            }
            else
                return null;
        }

        public async Task<IActionResult> Get(int id)
        {
            var httpClient = GetHttpClient("Users");
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
                User? user = await JsonSerializer.DeserializeAsync<User>(contentStream, options);

                return View(_service.Get(user));
            }
            else
                return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var httpClient = GetHttpClient("Users");
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
                IEnumerable<User>? users = await JsonSerializer.DeserializeAsync<IEnumerable<User>>(contentStream, options);
                if (users == null)
                    users = new List<User>();
                return View(_service.GetAll(users));
            }
            else
                return BadRequest();
        }
    }
}
