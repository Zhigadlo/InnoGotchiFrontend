﻿namespace InnoGotchi.DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte[] Avatar { get; set; }
        public Farm Farm { get; set; }
        public List<ColoborationRequest> SentRequests { get; set; }
        public List<ColoborationRequest> ReceivedRequests { get; set; }
    }
}
