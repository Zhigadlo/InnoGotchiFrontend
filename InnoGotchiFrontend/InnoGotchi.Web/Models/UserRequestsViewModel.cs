using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    public class UserRequestsViewModel
    {
        public List<KeyValuePair<int, UserDTO>> UsersWhoSentRequest { get; set; }
        public List<KeyValuePair<int, UserDTO>> UsersWhoReceivedRequest { get; set; }
    }
}
