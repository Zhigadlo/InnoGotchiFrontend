namespace InnoGotchi.DAL.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Appearance { get; set; }
        public int FarmId { get; set; }

        public int FeedingCount { get; set; }
        public int DrinkingCount { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime DeadTime { get; set; }
        public DateTime LastFeedingTime { get; set; }
        public DateTime LastDrinkingTime { get; set; }
        public DateTime FirstHappinessDate { get; set; }
    }
}
