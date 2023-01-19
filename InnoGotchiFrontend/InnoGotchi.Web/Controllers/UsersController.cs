using Hanssens.Net;
using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Identity;
using InnoGotchi.BLL.Services;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.Models;
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
                               ImageService imageService,
                               LocalStorage localStorage) : base(httpClientFactory, localStorage)
        {
            _userService = userService;
            _imageService = imageService;
        }
        
        public IActionResult Login(string errorMessage)
        {
            return View(errorMessage);
        }
        
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ChangePasswordView()
        {
            return View();
        }

        public async Task<IActionResult> Coloborators()
        {
            UserDTO? user = await GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login");

            List<UserDTO> coloborators = new List<UserDTO>();
            foreach(var rr in user.ReceivedRequests)
            {
                if (rr.IsConfirmed)
                {
                    var u = await Get(rr.RequestOwnerId);
                    coloborators.Add(u);
                }
            }
            foreach(var sr in user.SentRequests)
            {
                if (sr.IsConfirmed)
                {
                    var u = await Get(sr.RequestReceipientId);
                    coloborators.Add(u);
                }
            }
            return View(coloborators);
        }

        public async Task<IActionResult> UserRequests()
        {
            UserDTO? user = await GetCurrentUser();
            if(user == null) 
                return RedirectToAction("Login");
            List<KeyValuePair<int, UserDTO>> usersWhoSentRequest = new List<KeyValuePair<int, UserDTO>>();
            foreach (var rr in user.ReceivedRequests)
            {
                if (rr.IsConfirmed == false)
                {
                    var u = await Get(rr.RequestOwnerId);
                    
                    usersWhoSentRequest.Add(KeyValuePair.Create(rr.Id, u));
                }
            };
            List<KeyValuePair<int, UserDTO>> usersWhoReceivedRequest = new List<KeyValuePair<int, UserDTO>>();
            foreach(var sr in user.SentRequests)
            {
                if (sr.IsConfirmed == false)
                {
                    var u = await Get(sr.RequestReceipientId);
                    usersWhoReceivedRequest.Add(KeyValuePair.Create(sr.Id, u));
                }
            };
            UserRequestsViewModel vm = new UserRequestsViewModel()
            {
                UsersWhoSentRequest = usersWhoSentRequest,
                UsersWhoReceivedRequest = usersWhoReceivedRequest
            };
            return View(vm);
        }

        public async Task<IActionResult> AllUsers()
        {
            IEnumerable<UserDTO>? usersEnumerable = await GetAll();
            if (usersEnumerable == null)
                return RedirectToAction("Login");
            var users = usersEnumerable.ToList();
            int authorized_id = int.Parse(HttpContext.User.FindFirstValue(nameof(SecurityToken.UserId)));
            UserDTO authorizedUser = users.ToList().Find(u => u.Id == authorized_id);
            users.Remove(authorizedUser);
            List<UserViewModel> usersVM = new List<UserViewModel>();
            foreach(UserDTO user in users)
            {
                UserViewModel userVM = new UserViewModel
                {
                    User = user
                };

                var sentRequest = authorizedUser.SentRequests.FirstOrDefault(sr => sr.RequestReceipientId == user.Id);
                var receivedRequest = authorizedUser.ReceivedRequests.FirstOrDefault(sr => sr.RequestOwnerId == user.Id);
                
                if (sentRequest != null)
                {
                    if (sentRequest.IsConfirmed)
                        userVM.RequestState = RequestState.Confirmed;
                    else
                        userVM.RequestState = RequestState.Sent;

                    userVM.RequestId = sentRequest.Id;
                }
                else if (receivedRequest != null)
                {
                    if (receivedRequest.IsConfirmed)
                        userVM.RequestState = RequestState.Confirmed;
                    else
                        userVM.RequestState = RequestState.Received;

                    userVM.RequestId = receivedRequest.Id;
                }
                else
                    userVM.RequestState = RequestState.NotUsed;

                usersVM.Add(userVM);
            }
            return View(usersVM);
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
            UserDTO? user = await GetCurrentUser();
            if(user == null) 
                return RedirectToAction("Login");
            return View(user);
        }

        private async Task<UserDTO?> GetCurrentUser()
        {
            string userId = HttpContext.User.FindFirstValue(nameof(SecurityToken.UserId));
            if (userId == null)
            {
                return null;
            }
            return await Get(int.Parse(userId));
        }
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var httpClient = GetHttpClient("Users");

            var parameters = new Dictionary<string, string>();
            parameters["Id"] = HttpContext.User.FindFirstValue(nameof(SecurityToken.UserId));
            parameters["OldPassword"] = oldPassword;
            parameters["NewPassword"] = newPassword;
            parameters["ConfirmPassword"] = confirmPassword;

            var httpResponseMessage = await httpClient.PutAsync(httpClient.BaseAddress + "/passwordChange", new FormUrlEncodedContent(parameters));
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return await Logout();
            }
            else
                return View("ChangePasswordView", new ErrorModel { Error="Old password is wrong"});
        }
        public async Task<IActionResult> Logout()
        {
            _localStorage.Remove(nameof(SecurityToken));
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
                _localStorage.Store(nameof(SecurityToken), jsonToken);
                await SignIn(securityToken);
                return RedirectToAction("Index", "Home");
            }
            else
                return View("Login", new ErrorModel{ Error = "Invalid email or password" });
        }
        
        private async Task SignIn(SecurityToken securityToken)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, securityToken.Email),
                new Claim(ClaimTypes.Name, securityToken.UserName),
                new Claim(nameof(SecurityToken.AccessToken), securityToken.AccessToken),
                new Claim(nameof(SecurityToken.ExpireAt), securityToken.ExpireAt.ToString()),
                new Claim(nameof(SecurityToken.UserId), securityToken.UserId.ToString()),
                new Claim(nameof(SecurityToken.FarmId), securityToken.FarmId.ToString())
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
        public async Task<IEnumerable<UserDTO>?> GetAll()
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
                return _userService.GetAll(users);
            }

            return null;
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
            parameters["Id"] = HttpContext.User.FindFirstValue(nameof(SecurityToken.UserId));
            parameters["Avatar"] = Convert.ToBase64String(_imageService.GetBytesFromFormFile(FormFile));

            var httpResponseMessage = await httpClient.PutAsync(httpClient.BaseAddress + "/avatarChange", new FormUrlEncodedContent(parameters));
            return httpResponseMessage.IsSuccessStatusCode;
        }
    }
}
