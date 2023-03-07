using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    /// <summary>
    /// Model for AllPetsView
    /// </summary>
    public class AllPetsViewModel
    {
        public PaginatedListDTO<List<PetDTO>>? PaginatedList { get; set; }
        public PetSortModel? SortModel { get; set; }
        public FilterPetViewModel? FilterModel { get; set; }
    }
}
