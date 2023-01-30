namespace InnoGotchi.DAL.Models
{
    /// <summary>
    /// Entity for pet filtration
    /// </summary>
    public class PetFilterModel
    {
        public long Age { get; set; }
        public long GameYear { get; set; }

        public int HungerLavel { get; set; }
        public long FeedingPeriod { get; set; }
        public bool IsLastHungerStage { get; set; }

        public int ThirstyLavel { get; set; }
        public long DrinkingPeriod { get; set; }
        public bool IsLastThirstyStage { get; set; }
    }
}
