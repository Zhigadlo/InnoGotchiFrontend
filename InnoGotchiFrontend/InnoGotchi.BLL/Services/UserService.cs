using AutoMapper;
using Hanssens.Net;
using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Identity;
using InnoGotchi.DAL.Managers;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.BLL.Services
{
    /// <summary>
    /// Class that have ability to get users data from data access layer
    /// </summary>
    public class UserService : BaseService
    {
        private UserManager _userManager;
        public UserService(IMapper mapper,
                           IHttpClientFactory httpClientFactory,
                           LocalStorage localStorage,
                          IConfiguration configuration) : base(mapper)
        {
            _userManager = new UserManager(httpClientFactory, localStorage, configuration);
        }
        /// <summary>
        /// Gets user by id
        /// </summary>
        public async Task<UserDTO?> Get(int id)
        {
            var user = await _userManager.Get(id);
            return _mapper.Map<UserDTO>(user);
        }
        /// <summary>
        /// Gets all pets
        /// </summary>
        public async Task<IEnumerable<UserDTO>?> GetAll()
        {
            var users = await _userManager.GetAll();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }
        /// <summary>
        /// Gets all emails that already in use
        /// </summary>
        public async Task<IEnumerable<string>?> GetAllEmails()
        {
            return await _userManager.GetAllEmails();
        }
        /// <summary>
        /// Creates new user
        /// </summary>
        public async Task<int> Create(UserDTO? user)
        {
            return await _userManager.Create(_mapper.Map<User>(user));
        }
        /// <summary>
        /// Updates user avatar
        /// </summary>
        public async Task<bool> AvatarUpdate(int id, byte[] appearance)
        {
            return await _userManager.AvatarUpdate(id, appearance);
        }
        /// <summary>
        /// Authenticates user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<SecurityToken?> Authenticate(string email, string password)
        {
            var token = await _userManager.Authenticate(email, password);
            return _mapper.Map<SecurityToken>(token);
        }

        public void RemoveTokenFromLocalStorage()
        {
            _userManager.RemoveTokenFromLocalStorage();
        }
        /// <summary>
        /// Gets all coloborators by user id
        /// </summary>
        /// <param name="id">User id</param>
        public async Task<IEnumerable<UserDTO>?> Coloborators(int id)
        {
            var coloborators = await _userManager.Coloborators(id);
            return _mapper.Map<IEnumerable<UserDTO>>(coloborators);
        }
        /// <summary>
        /// Gets all users that sent request to user by id
        /// </summary>
        /// <param name="id">User that receved requests id</param>
        public async Task<IEnumerable<UserDTO>?> SentRequestUsers(int id)
        {
            var sentRequestUsers = await _userManager.SentRequestUsers(id);
            return _mapper.Map<IEnumerable<UserDTO>>(sentRequestUsers);
        }
        /// <summary>
        /// Gets all users that received requests to user by id
        /// </summary>
        /// <param name="id">User that sent id</param>
        public async Task<IEnumerable<UserDTO>?> ReceivedRequestUsers(int id)
        {
            var receivedRequestUsers = await _userManager.ReceivedRequestUsers(id);
            return _mapper.Map<IEnumerable<UserDTO>>(receivedRequestUsers);
        }
        /// <summary>
        /// Updates user password 
        /// </summary>
        public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword, string confirm)
        {
            return await _userManager.ChangePassword(userId, oldPassword, newPassword, confirm);
        }
    }
}
