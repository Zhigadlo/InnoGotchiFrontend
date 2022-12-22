using InnoGotchi.DAL.Models;

namespace InnoGotchi.Web.Models
{
    public class AuthorizedUserModel
    {
        public string? AccessToken { get; set; }
        public User? User { get; set; }
    }
}
