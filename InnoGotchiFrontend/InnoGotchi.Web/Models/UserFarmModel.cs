using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    /// <summary>
    /// Model for UserFarm view
    /// </summary>
    public class UserFarmModel
    {
        public FarmDTO? Farm { get; set; }
        public int AuthorizedUserId { get; set; }
        public IEnumerable<string>? FarmNames { get; set; }
    }
}
