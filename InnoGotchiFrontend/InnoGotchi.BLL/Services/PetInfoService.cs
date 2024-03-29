﻿using InnoGotchi.BLL.DTO;
using InnoGotchi.DAL.Managers;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.BLL.Services
{
    /// <summary>
    /// Class that manage info about pets
    /// </summary>
    public class PetInfoService
    {
        public DateTime FeedingPeriodHours { get; private set; }
        public DateTime DrinkingPeriodHours { get; private set; }
        public DateTime DayHours { get; private set; }
        public DateTime GameYear { get; private set; }

        public PetInfoService(IConfiguration configuration)
        {
            GameYear = DateTime.MinValue.AddDays(int.Parse(configuration.GetSection("GameYearDays").Value));
            DayHours = DateTime.MinValue.AddTicks(GameYear.Ticks / 365);
            FeedingPeriodHours = DateTime.MinValue.AddDays(int.Parse(configuration.GetSection("FeedingsPerRealDay").Value));
            DrinkingPeriodHours = DateTime.MinValue.AddDays(int.Parse(configuration.GetSection("DrinkingPerRealDay").Value));
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
        public bool IsDeath(PetDTO pet)
        {
            long deathTime = pet.LastDrinkingTime.Ticks + DrinkingPeriodHours.Ticks * (int)ThirstyLavel.Dead;
            if (DateTime.UtcNow.Ticks >= deathTime)
            {
                pet.DeadTime = DateTime.MinValue.AddTicks(deathTime);
                return true;
            }
            deathTime = pet.LastFeedingTime.Ticks + FeedingPeriodHours.Ticks * (int)HungerLavel.Dead;
            if (DateTime.UtcNow.Ticks >= deathTime)
            {
                pet.DeadTime = DateTime.MinValue.AddTicks(deathTime);
                return true;
            }
            return false;
        }

        private int GetAge(PetDTO pet)
        {
            if (GetPetState(pet) != PetState.Dead)
                return (int)((DateTime.UtcNow - pet.CreateTime).Ticks / GameYear.Ticks);

            if (pet.LastDrinkingTime > pet.LastFeedingTime)
            {
                pet.DeadTime = pet.LastFeedingTime.AddTicks(FeedingPeriodHours.Ticks * (int)HungerLavel.Dead);
            }
            else
            {
                pet.DeadTime = pet.LastDrinkingTime.AddTicks(DrinkingPeriodHours.Ticks * (int)ThirstyLavel.Dead);
            }
            return (int)((pet.DeadTime - pet.CreateTime).Ticks / GameYear.Ticks);
        }

        private PetState GetPetState(PetDTO pet)
        {
            if (GetHungryLavel(pet) == HungerLavel.Dead
                || GetThirstyLavel(pet) == ThirstyLavel.Dead)
                return PetState.Dead;

            if ((DateTime.UtcNow - pet.CreateTime).Ticks < DateTime.MinValue.AddDays(2).Ticks)
                return PetState.Newborn;
            
            return PetState.Alive;
        }

        private HungerLavel GetHungryLavel(PetDTO pet)
        {
            if ((DateTime.UtcNow - pet.LastFeedingTime).Ticks > FeedingPeriodHours.Ticks * (int)HungerLavel.Dead)
                return HungerLavel.Dead;
            if ((DateTime.UtcNow - pet.LastFeedingTime).Ticks > FeedingPeriodHours.Ticks * (int)HungerLavel.Hungry)
                return HungerLavel.Hungry;
            if ((DateTime.UtcNow - pet.LastFeedingTime).Ticks > FeedingPeriodHours.Ticks * (int)HungerLavel.Normal)
                return HungerLavel.Normal;
            
            return HungerLavel.Full;
        }
        private ThirstyLavel GetThirstyLavel(PetDTO pet)
        {
            if ((DateTime.UtcNow - pet.LastDrinkingTime).Ticks > DrinkingPeriodHours.Ticks * (int)ThirstyLavel.Dead)
                return ThirstyLavel.Dead;
            if ((DateTime.UtcNow - pet.LastDrinkingTime).Ticks > DrinkingPeriodHours.Ticks * (int)ThirstyLavel.Thirsty)
                return ThirstyLavel.Thirsty;
            if ((DateTime.UtcNow - pet.LastDrinkingTime).Ticks > DrinkingPeriodHours.Ticks * (int)ThirstyLavel.Normal)
                return ThirstyLavel.Normal;
            
            return ThirstyLavel.Full;
        }

        private int GetHappinessDaysCount(PetDTO pet)
        {
            if (!pet.IsAlive)
                return 0;
            
            return (int)((DateTime.UtcNow - pet.FirstHappinessDate).Ticks / DayHours.Ticks * 1.0);
        }

        private double GetAverageFeedingPeriod(PetDTO pet)
        {
            TimeSpan lifeTime = (pet.DeadTime - pet.CreateTime);

            if (pet.IsAlive)
                lifeTime = (DateTime.UtcNow - pet.CreateTime);

            int petDays = (int)(lifeTime.Ticks / DayHours.Ticks);
            return petDays != 0 ? pet.FeedingCount / petDays : pet.FeedingCount;
        }

        private double GetAverageDrinkingPeriod(PetDTO pet)
        {
            TimeSpan lifeTime = (pet.DeadTime - pet.CreateTime);

            if (pet.IsAlive)
                lifeTime = (DateTime.UtcNow - pet.CreateTime);

            int petDays = (int)(lifeTime.Ticks / DayHours.Ticks);
            return lifeTime.Days != 0 ? pet.DrinkingCount / petDays : pet.DrinkingCount;
        }
    }
}
