﻿using AutoMapper;
using Hanssens.Net;
using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Identity;
using InnoGotchi.DAL.Managers;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.BLL.Services
{
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



        public async Task<UserDTO?> Get(int id)
        {
            var user = await _userManager.Get(id);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<IEnumerable<UserDTO>?> GetAll()
        {
            var users = await _userManager.GetAll();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<IEnumerable<string>?> GetAllEmails()
        {
            return await _userManager.GetAllEmails();
        }

        public async Task<int> Create(UserDTO? user)
        {
            return await _userManager.Create(_mapper.Map<User>(user));
        }

        public async Task<bool> AvatarUpdate(int id, byte[] appearance)
        {
            return await _userManager.AvatarUpdate(id, appearance);
        }

        public async Task<SecurityToken?> Authenticate(string email, string password)
        {
            var token = await _userManager.Authenticate(email, password);
            return _mapper.Map<SecurityToken>(token);
        }
        public void RemoveTokenFromLocalStorage()
        {
            _userManager.RemoveTokenFromLocalStorage();
        }

        public async Task<IEnumerable<UserDTO>?> Coloborators(int id)
        {
            var coloborators = await _userManager.Coloborators(id);
            return _mapper.Map<IEnumerable<UserDTO>>(coloborators);
        }

        public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword, string confirm)
        {
            return await _userManager.ChangePassword(userId, oldPassword, newPassword, confirm);
        }
    }
}
