﻿using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    public class UserViewModel
    {
        public UserDTO User { get; set; }
        public RequestState RequestState { get; set; }
        public int RequestId { get; set; }
    }

    public enum RequestState
    {
        NotUsed,
        Sent,
        Received,
        Confirmed
    }
}
