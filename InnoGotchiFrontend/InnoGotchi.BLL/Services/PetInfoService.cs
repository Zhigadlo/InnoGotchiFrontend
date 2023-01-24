using InnoGotchi.BLL.DTO;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.BLL.Services
{
    public class PetInfoService
    {
        public DateTime FeedingPeriodHours { get; private set; }
        public DateTime DrinkingPeriodHours { get; private set; }
        public DateTime DayHours { get; private set; }

        public PetInfoService(IConfiguration configuration)
        {
            DayHours = DateTime.MinValue.AddHours(int.Parse(configuration.GetSection("DayHours").Value));
            FeedingPeriodHours = DateTime.MinValue.AddHours(int.Parse(configuration.GetSection("FeedingPeriodHours").Value));
            DrinkingPeriodHours = DateTime.MinValue.AddHours(int.Parse(configuration.GetSection("DrinkingPeriodHours").Value));
        }

        public PetDTO FillPetDTO(PetDTO petDTO)
        {
            petDTO.State = GetPetState(petDTO);
            petDTO.Age = GetAge(petDTO);
            petDTO.HungerLavel = GetHungryLavel(petDTO);
            petDTO.ThirstyLavel = GetThirstyLavel(petDTO);
            petDTO.HappinessDaysCount = GetHappinessDaysCount(petDTO);
            petDTO.AverageDrinkingPeriod = GetAverageDrinkingPeriod(petDTO);
            petDTO.AverageFeedingPeriod = GetAverageFeedingPeriod(petDTO);
            return petDTO;
        }
        private int GetAge(PetDTO pet)
        {
            if (GetPetState(pet) != PetState.Dead)
                return (int)((DateTime.UtcNow - pet.CreateTime).Ticks / DayHours.Ticks / 365);
            else
            {
                if (pet.LastDrinkingTime > pet.LastFeedingTime)
                {
                    pet.DeadTime = pet.LastFeedingTime.AddTicks(FeedingPeriodHours.Ticks * 3);
                }
                else
                {
                    pet.DeadTime = pet.LastDrinkingTime.AddTicks(DrinkingPeriodHours.Ticks * 3);
                }
                return (int)((pet.DeadTime - pet.CreateTime).Ticks / DayHours.Ticks / 365);
            }
        }

        private PetState GetPetState(PetDTO pet)
        {
            if (GetHungryLavel(pet) != HungerLavel.Dead
                && GetThirstyLavel(pet) != ThirstyLavel.Dead)
                return PetState.Alive;
            else
                return PetState.Dead;
        }

        private HungerLavel GetHungryLavel(PetDTO pet)
        {
            if ((DateTime.UtcNow - pet.LastFeedingTime).Ticks > FeedingPeriodHours.Ticks * 3)
                return HungerLavel.Dead;
            else if ((DateTime.UtcNow - pet.LastFeedingTime).Ticks > FeedingPeriodHours.Ticks * 2)
                return HungerLavel.Hungry;
            else if ((DateTime.UtcNow - pet.LastFeedingTime).Ticks > FeedingPeriodHours.Ticks)
                return HungerLavel.Normal;
            else
                return HungerLavel.Full;
        }
        private ThirstyLavel GetThirstyLavel(PetDTO pet)
        {
            if ((DateTime.UtcNow - pet.LastDrinkingTime).Ticks > DrinkingPeriodHours.Ticks * 3)
                return ThirstyLavel.Dead;
            else if ((DateTime.UtcNow - pet.LastDrinkingTime).Ticks > DrinkingPeriodHours.Ticks * 2)
                return ThirstyLavel.Thirsty;
            else if ((DateTime.UtcNow - pet.LastDrinkingTime).Ticks > DrinkingPeriodHours.Ticks)
                return ThirstyLavel.Normal;
            else
                return ThirstyLavel.Full;
        }

        private int GetHappinessDaysCount(PetDTO pet)
        {
            return (int)((DateTime.UtcNow - pet.FirstHappinessDate).Ticks / DayHours.Ticks);
        }

        private double GetAverageFeedingPeriod(PetDTO pet)
        {
            return (DateTime.UtcNow - pet.CreateTime).Ticks / pet.FeedingCount / DayHours.Ticks;
        }

        private double GetAverageDrinkingPeriod(PetDTO pet)
        {
            return (DateTime.UtcNow - pet.CreateTime).Ticks / pet.DrinkingCount / DayHours.Ticks;
        }
    }
}
