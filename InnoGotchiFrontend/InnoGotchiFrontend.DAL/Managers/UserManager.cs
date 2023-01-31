using Hanssens.Net;
using InnoGotchi.DAL.Identity;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace InnoGotchi.DAL.Managers
{
    /// <summary>
    /// Class that have access to user entities from server
    /// </summary>
    public class UserManager : BaseManager
    {
        public UserManager(IHttpClientFactory httpClientFactory,
                           LocalStorage localStorage,
                          IConfiguration configuration) : base(httpClientFactory, localStorage, configuration)
        {
        }
        /// <summary>
        /// Gets all emails that already in use from server
        /// </summary>
        public async Task<IEnumerable<string>?> GetAllEmails()
        {
            var httpClient = GetHttpClient("Users");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + "/getAllEmails"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                IEnumerable<string>? emails = await JsonSerializer.DeserializeAsync<IEnumerable<string>>(contentStream, options);
                if (emails == null)
                    emails = new List<string>();
                return emails;
            }

            return null;
        }
        /// <summary>
        /// Gets all users from server
        /// </summary>
        public async Task<IEnumerable<User>?> GetAll()
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
                return users;
            }

            return null;
        }
        /// <summary>
        /// Creates user 
        /// </summary>
        public async Task<int> Create(User? user)
        {
            var httpClient = GetHttpClient("Users");

            var parameters = new Dictionary<string, string>();
            parameters["FirstName"] = user.FirstName;
            parameters["LastName"] = user.LastName;
            parameters["Password"] = user.Password;
            parameters["Email"] = user.Email;
            parameters["Avatar"] = Convert.ToBase64String(user.Avatar);

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                int userId = await JsonSerializer.DeserializeAsync<int>(contentStream, options);

                return userId;
            }
            else
                return -1;
        }
        /// <summary>
        /// Updates user avatar
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newavatar"></param>
        public async Task<bool> AvatarUpdate(int userId, byte[] newavatar)
        {
            var httpClient = GetHttpClient("Users");

            var parameters = new Dictionary<string, string>();
            parameters["Id"] = userId.ToString();
            parameters["Avatar"] = Convert.ToBase64String(newavatar);

            var httpResponseMessage = await httpClient.PutAsync(httpClient.BaseAddress + "/avatarChange", new FormUrlEncodedContent(parameters));
            return httpResponseMessage.IsSuccessStatusCode;
        }
        /// <summary>
        /// Gets user jwt token by email and password
        /// </summary>
        private async Task<SecurityTokenModel?> Token(string email, string password)
        {
            var httpClient = GetHttpClient("Users");
            var parameters = new Dictionary<string, string>();
            parameters["email"] = email;
            parameters["password"] = password;

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress + "/token", new FormUrlEncodedContent(parameters));

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                SecurityTokenModel? token = await JsonSerializer.DeserializeAsync<SecurityTokenModel>(contentStream, options);
                return token;
            }

            return null;
        }
        /// <summary>
        /// Authenticates user
        /// </summary>
        public async Task<SecurityTokenModel?> Authenticate(string email, string password)
        {
            var token = await Token(email, password);
            if (token != null)
            {
                var user = await GetUser(email, password);

                string jsonToken = JsonSerializer.Serialize(token);
                _localStorage.Store(_tokenName, jsonToken);
                return token;
            }
            else
                return null;
        }
        /// <summary>
        /// Removes jwt token from local storage
        /// </summary>
        public void RemoveTokenFromLocalStorage()
        {
            _localStorage.Remove(_tokenName);
        }
        /// <summary>
        /// Gets all coloborators by user id from server
        /// </summary>
        public async Task<IEnumerable<User>?> Coloborators(int id)
        {
            var httpClient = GetHttpClient("Users");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + $"/coloborators/{id}"
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

                return users;
            }
            else
                return null;
        }
        /// <summary>
        /// Gets all users that sent request to user by id from server
        /// </summary>
        /// <param name="id">User id</param>
        public async Task<IEnumerable<User>?> SentRequestUsers(int id)
        {
            var httpClient = GetHttpClient("Users");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + $"/sentRequestUsers/{id}"
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

                return users;
            }
            else
                return null;
        }
        /// <summary>
        /// Gets all users that received request from user by id from server
        /// </summary>
        /// <param name="id">User id</param>
        public async Task<IEnumerable<User>?> ReceivedRequestUsers(int id)
        {
            var httpClient = GetHttpClient("Users");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + $"/receivedRequestUsers/{id}"
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

                return users;
            }
            else
                return null;
        }
        /// <summary>
        /// Gets user from server by email and password
        /// </summary>
        private async Task<User?> GetUser(string email, string password)
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
                return user;
            }
            else
                return null;
        }
        /// <summary>
        /// Gets user from server by id
        /// </summary>
        public async Task<User?> Get(int id)
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

                return user;
            }
            else
                return null;
        }
        /// <summary>
        /// Updates user password
        /// </summary>
        public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword, string confirmPassword)
        {
            var httpClient = GetHttpClient("Users");

            var parameters = new Dictionary<string, string>();
            parameters["Id"] = userId.ToString();
            parameters["OldPassword"] = oldPassword;
            parameters["NewPassword"] = newPassword;
            parameters["ConfirmPassword"] = confirmPassword;

            var httpResponseMessage = await httpClient.PutAsync(httpClient.BaseAddress + "/passwordChange", new FormUrlEncodedContent(parameters));
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            else
                return false;
        }
    }
}
