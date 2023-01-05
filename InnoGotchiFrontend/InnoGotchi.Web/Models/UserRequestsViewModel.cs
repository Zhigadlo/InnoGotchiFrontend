using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    public class UserRequestsViewModel
    {
        public IEnumerable<ColoborationRequestDTO>? SentRequests { get; set; }
        public IEnumerable<ColoborationRequestDTO>? ReceivedRequests { get; set; }
    }
}
