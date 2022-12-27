using InnoGotchi.DAL.Models;

namespace InnoGotchi.Web.BLL.DTO
{
    public class FarmDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public List<PetDTO> Pets { get; set; }
        public int OwnerId { get; set; }
        public FarmDTO Farm { get; set; }
    }
}
