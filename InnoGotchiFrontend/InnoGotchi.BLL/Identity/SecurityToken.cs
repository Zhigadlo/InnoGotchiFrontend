namespace InnoGotchi.BLL.Identity
{
    public class SecurityToken
    {
        public string? AccessToken { get; set; }
        public int UserId { get; set; }
        public int FarmId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
