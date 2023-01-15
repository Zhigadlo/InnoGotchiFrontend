using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    public class UserRequestsViewModel
    {
        public List<UserDTO> UsersWhoSentRequest { get; set; }
        public List<UserDTO> UsersWhoReceivedRequest { get; set; }
    }
}
