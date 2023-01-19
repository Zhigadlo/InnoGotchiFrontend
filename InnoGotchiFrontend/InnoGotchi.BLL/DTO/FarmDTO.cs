namespace InnoGotchi.BLL.DTO
{
    public class FarmDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public List<PetDTO> Pets { get; set; }
        public int OwnerId { get; set; }
        public FarmDTO? Farm { get; set; }


        public double GetAverageHappinessDays => Pets.Count() == 0 ? 0 : Pets.Average(p => p.HappinessDaysCount);
        public double GetAveragePetsAge => Pets.Count() == 0 ? 0 : Pets.Average(p => p.Age);

        public double GetAverageFeedingPeriod => Pets.Count() == 0 ? 0 : Pets.Average(p => p.AverageFeedingPeriod);
        public double GetAverageDrinkingPeriod => Pets.Count() == 0 ? 0 : Pets.Average(p => p.AverageDrinkingPeriod);

        public int GetAlivePetsCount => Pets.Count(p => p.State != PetState.Dead);
        public int GetDeadPetsCount => Pets.Count(p => p.State == PetState.Dead);
    }
}
