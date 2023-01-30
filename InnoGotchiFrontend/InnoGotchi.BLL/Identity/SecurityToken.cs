namespace InnoGotchi.BLL.Identity
{
    /// <summary>
    /// Entity that contains user jwt token and other authorized user information
    /// </summary>
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
