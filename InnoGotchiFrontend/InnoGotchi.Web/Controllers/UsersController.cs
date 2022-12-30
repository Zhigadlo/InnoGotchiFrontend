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
        private UserService _userService;
        private ImageService _imageService;
        public UsersController(IHttpClientFactory httpClientFactory,
                               UserService userService,
                               ImageService imageService) : base(httpClientFactory)
        {
            _userService = userService;
            _imageService = imageService;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ChangePasswordView()
        {
            return View();
        }

        public async Task<IActionResult> ChangeAvatar(IFormFile FormFile)
        {
            bool result = await AvatarUpdate(FormFile);
            if (result)
                return RedirectToAction("UserProfile");
            else
                return BadRequest();
        }

        public async Task<IActionResult> UserProfile()
        {
            int userId = int.Parse(HttpContext.User.FindFirstValue("user_id"));
            UserDTO? user = await Get(userId);
            return View(user);
        }


        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var httpClient = GetHttpClient("Users");

            var parameters = new Dictionary<string, string>();
            parameters["Id"] = HttpContext.User.FindFirstValue("user_id");
            parameters["OldPassword"] = oldPassword;
            parameters["NewPassword"] = newPassword;
            parameters["ConfirmPassword"] = confirmPassword;

            var httpResponseMessage = await httpClient.PutAsync(httpClient.BaseAddress + "/passwordChange", new FormUrlEncodedContent(parameters));
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return await Logout();
            }
            else
                return BadRequest();
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Delete(_securityTokenKey);
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
                    ExpireAt = DateTime.UtcNow.AddHours(1),
                    FarmId = user.Farm == null ? -1 : user.Farm.Id
                };
                string jsonToken = JsonSerializer.Serialize(securityToken);
                HttpContext.Response.Cookies.Append(_securityTokenKey, jsonToken);
                await SignIn(securityToken);
                return RedirectToAction("Index", "Home");
            }
            else
                return BadRequest();
        }
        private async Task SignIn(SecurityToken securityToken)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, securityToken.Email),
                new Claim(ClaimTypes.Name, securityToken.UserName),
                new Claim("access_token", securityToken.AccessToken),
                new Claim("expiredAt", securityToken.ExpireAt.ToString()),
                new Claim("user_id", securityToken.UserId.ToString()),
                new Claim("farm_id", securityToken.FarmId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(claimsPrincipal);
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
                return _userService.GetUserDTO(user);
            }
            else
                return null;
        }
        public async Task<UserDTO?> Get(int id)
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

                return _userService.GetUserDTO(user);
            }
            else
                return null;
        }
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
                return View(_userService.GetAll(users));
            }
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO? user)
        {
            User? person = _userService.GetUser(user);
            person.Avatar = _imageService.GetBytesFromFormFile(user.FormFile);

            var httpClient = GetHttpClient("Users");

            var parameters = new Dictionary<string, string>();
            parameters["FirstName"] = person.FirstName;
            parameters["LastName"] = person.LastName;
            parameters["Password"] = person.Password;
            parameters["Email"] = person.Email;
            parameters["Avatar"] = Convert.ToBase64String(person.Avatar);

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }
            else
                return BadRequest();
        }

        [HttpPut]
        public async Task<bool> AvatarUpdate(IFormFile FormFile)
        {
            var httpClient = GetHttpClient("Users");

            var parameters = new Dictionary<string, string>();
            parameters["Id"] = HttpContext.User.FindFirstValue("user_id");
            parameters["Avatar"] = Convert.ToBase64String(_imageService.GetBytesFromFormFile(FormFile));

            var httpResponseMessage = await httpClient.PutAsync(httpClient.BaseAddress + "/avatarChange", new FormUrlEncodedContent(parameters));
            return httpResponseMessage.IsSuccessStatusCode;
        }
    }
}
