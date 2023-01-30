using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    /// <summary>
    /// Model for PetContructorView
    /// </summary>
    public class PetConstructorViewModel
    {
        public List<PictureDTO> Bodies { get; set; }
        public List<PictureDTO> Eyes { get; set; }
        public List<PictureDTO> Noses { get; set; }
        public List<PictureDTO> Mouths { get; set; }
        public IEnumerable<string> PetNames { get; set; }

    }
}
