using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    /// <summary>
    /// Model for PetContructorView
    /// </summary>
    public class PetConstructorViewModel
    {
        public IEnumerable<PictureDTO> Bodies { get; set; }
        public IEnumerable<PictureDTO> Eyes { get; set; }
        public IEnumerable<PictureDTO> Noses { get; set; }
        public IEnumerable<PictureDTO> Mouths { get; set; }
        public IEnumerable<string> PetNames { get; set; }

    }
}
