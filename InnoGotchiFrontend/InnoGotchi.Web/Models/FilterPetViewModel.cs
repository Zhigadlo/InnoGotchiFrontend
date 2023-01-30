namespace InnoGotchi.Web.Models
{
    /// <summary>
    /// Model for set filtration states on AllPetsView
    /// </summary>
    public class FilterPetViewModel
    {
        public string SelectedAge { get; private set; }
        public int HungerLavel { get; private set; }
        public int ThirstyLavel { get; private set; }
        public FilterPetViewModel(string selectedAge, int hungerLavel, int thirstyLavel)
        {
            SelectedAge = selectedAge;
            HungerLavel = hungerLavel;
            ThirstyLavel = thirstyLavel;
        }
    }
}
