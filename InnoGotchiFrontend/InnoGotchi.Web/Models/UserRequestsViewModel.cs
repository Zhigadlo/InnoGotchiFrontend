using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    /// <summary>
    /// Model for UserRequests view
    /// </summary>
    public class UserRequestsViewModel
    {
        public List<KeyValuePair<int, UserDTO>> UsersWhoSentRequest { get; set; }
        public List<KeyValuePair<int, UserDTO>> UsersWhoReceivedRequest { get; set; }
    }
}
