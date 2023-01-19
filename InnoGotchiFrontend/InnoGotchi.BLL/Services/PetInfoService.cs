using InnoGotchi.BLL.DTO;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoGotchi.BLL.Services
{
    public class PetInfoService
    {
        private DateTime _feedingPeriodHours;
        private DateTime _drinkingPeriodHours;
        private DateTime _dayHours;

        public PetInfoService(IConfiguration configuration)
        {
            _dayHours = DateTime.MinValue.AddHours(int.Parse(configuration.GetSection("DayHours").Value));
            _feedingPeriodHours = DateTime.MinValue.AddHours(int.Parse(configuration.GetSection("FeedingPeriodHours").Value));
            _drinkingPeriodHours = DateTime.MinValue.AddHours(int.Parse(configuration.GetSection("DrinkingPeriodHours").Value));
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
                return (int)((DateTime.UtcNow - pet.CreateTime).Ticks / _dayHours.Ticks / 365);
            else
            {
                if (pet.LastDrinkingTime > pet.LastFeedingTime)
                {
                    pet.DeadTime = pet.LastFeedingTime.AddTicks(_feedingPeriodHours.Ticks * 3);
                }
                else
                {
                    pet.DeadTime = pet.LastDrinkingTime.AddTicks(_drinkingPeriodHours.Ticks * 3);
                }
                return (int)((pet.DeadTime - pet.CreateTime).Ticks / _dayHours.Ticks / 365);
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
            if ((DateTime.UtcNow - pet.LastFeedingTime).Ticks > _feedingPeriodHours.Ticks * 3)
                return HungerLavel.Dead;
            else if ((DateTime.UtcNow - pet.LastFeedingTime).Ticks > _feedingPeriodHours.Ticks * 2)
                return HungerLavel.Hungry;
            else if ((DateTime.UtcNow - pet.LastFeedingTime).Ticks > _feedingPeriodHours.Ticks)
                return HungerLavel.Normal;
            else
                return HungerLavel.Full;
        }
        private ThirstyLavel GetThirstyLavel(PetDTO pet)
        {
            if ((DateTime.UtcNow - pet.LastDrinkingTime).Ticks > _drinkingPeriodHours.Ticks * 3)
                return ThirstyLavel.Dead;
            else if ((DateTime.UtcNow - pet.LastDrinkingTime).Ticks > _drinkingPeriodHours.Ticks * 2)
                return ThirstyLavel.Thirsty;
            else if ((DateTime.UtcNow - pet.LastDrinkingTime).Ticks > _drinkingPeriodHours.Ticks)
                return ThirstyLavel.Normal;
            else
                return ThirstyLavel.Full;
        }

        private int GetHappinessDaysCount(PetDTO pet)
        {
            return (int)((DateTime.UtcNow - pet.FirstHappinessDate).Ticks / _dayHours.Ticks);
        }

        private double GetAverageFeedingPeriod(PetDTO pet)
        {
            return (DateTime.UtcNow - pet.CreateTime).Ticks / pet.FeedingCount / _dayHours.Ticks;
        }

        private double GetAverageDrinkingPeriod(PetDTO pet)
        {
            return (DateTime.UtcNow - pet.CreateTime).Ticks / pet.DrinkingCount / _dayHours.Ticks;
        }
    }
}
