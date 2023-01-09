using InnoGotchi.BLL.DTO;
using static System.Net.Mime.MediaTypeNames;

namespace InnoGotchi.Web.Models
{
    public class PetConstructorViewModel
    {
        public List<PictureDTO> Bodies { get; set; }
        public List<PictureDTO> Eyes { get; set; }
        public List<PictureDTO> Noses { get; set; }
        public List<PictureDTO> Mouths { get; set; }
    }
}
