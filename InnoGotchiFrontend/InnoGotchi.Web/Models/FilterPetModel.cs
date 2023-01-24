using InnoGotchi.BLL.DTO;

namespace InnoGotchi.Web.Models
{
    public class FilterPetModel
    {
        public string SelectedAge { get; private set; }
        public int HungerLavel { get; private set; }
        public int ThirstyLavel { get; private set; }
        public FilterPetModel(string selectedAge, int hungerLavel, int thirstyLavel)
        {
            SelectedAge = selectedAge;
            HungerLavel = hungerLavel;
            ThirstyLavel = thirstyLavel;
        }
    }
}
