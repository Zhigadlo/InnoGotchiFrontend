namespace InnoGotchi.Web.Models
{
    public class PetSortModel
    {
        public string NameState { get; private set; }
        public string AgeState { get; private set; }
        public string PetState { get; private set; }
        public string HungryState { get; private set; }
        public string ThirstyState { get; private set; }
        public string HappinessState { get; private set; }

        public string Current { get; private set; }

        public PetSortModel(string sortOrder)
        {
            NameState = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            AgeState = sortOrder == "age_asc" ? "age_desc" : "age_asc";
            PetState = sortOrder == "state_asc" ? "state_desc" : "state_asc";
            HungryState = sortOrder == "hunger_asc" ? "hunger_desc" : "hunger_asc";
            ThirstyState = sortOrder == "thirsty_asc" ? "thirsty_desc" : "thirsty_asc";
            HappinessState = sortOrder == "happiness_asc" ? "happiness_desc" : "happiness_asc";

            Current = sortOrder;
        }
    }
}
