﻿using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    /// <summary>
    /// Model for UserRequests view
    /// </summary>
    public class UserRequestsViewModel
    {
        public List<UserDTO> UsersWhoSentRequest { get; set; }
        public List<UserDTO> UsersWhoReceivedRequest { get; set; }
    }
}
