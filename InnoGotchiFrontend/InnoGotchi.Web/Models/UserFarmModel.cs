using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    public class UserFarmModel
    {
        public FarmDTO Farm { get; set; }
        public int AuthorizedUserId { get; set; }
    }
}
