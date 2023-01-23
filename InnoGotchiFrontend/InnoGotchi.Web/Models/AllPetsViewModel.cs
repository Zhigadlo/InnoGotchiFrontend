using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Models;

namespace InnoGotchi.Web.Models
{
    public class AllPetsViewModel
    {
        public PaginatedList<PetDTO> PaginatedList { get; set; }
        public PetSortModel SortModel { get; set; }
    }
}
