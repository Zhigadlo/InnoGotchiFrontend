using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Models;
using InnoGotchi.BLL.Services;
using InnoGotchi.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace InnoGotchi.Web.Controllers
{
    public class PetsController : BaseController
    {
        private PetService _petService;
        private PetInfoService _petInfoService;
        public PetsController(PetService petService,
                              PetInfoService petInfoService)
        {
            _petService = petService;
            _petInfoService = petInfoService;
        }

        public async Task<IActionResult> PetView(int id)
        {
            var pet = await Get(id);
            return View(pet);
        }

        public async Task<PetDTO?> Get(int id)
        {
            return await _petService.Get(id);
        }

        public async Task<PaginatedListDTO<PetDTO>?> GetPage(int page, string sortType, PetFilterModelDTO filterModel)
        {
            var pets = await _petService.GetPage(page, sortType, filterModel);
            if (pets == null)
                pets = new PaginatedListDTO<PetDTO>();
            else
                pets.Items?.ForEach(async p =>
                {
                    if (_petInfoService.IsDeath(p))
                        await _petService.Death(p.Id, p.DeadTime.Ticks);
                    //await IsDeath(p);
                    _petInfoService.FillPetDTO(p);
                });

            return pets;
        }

        private string GetStringFromSession(string key, string queryName, string defaultValue = "")
        {
            if (HttpContext.Request.Query[queryName].Count() > 0)
            {
                return HttpContext.Request.Query[queryName][0];
            }
            else if (HttpContext.Session.Keys.Contains(key))
            {
                return HttpContext.Session.GetString(key);
            }
            else
            {
                return defaultValue;
            }
        }

        public async Task<IActionResult> AllPetsView(int page = 1, string sortType = "happiness_asc")
        {
            string age = GetStringFromSession("filterage", "age");
            HttpContext.Session.SetString("filterage", age);

            string hungerLavel = GetStringFromSession("filterhunger", "hungerLavel");
            HttpContext.Session.SetString("filterhunger", hungerLavel);
            hungerLavel = String.IsNullOrEmpty(hungerLavel) ? "-1" : hungerLavel;

            string thirstyLavel = GetStringFromSession("filterthirsty", "thirstyLavel");
            HttpContext.Session.SetString("filterthirsty", thirstyLavel);
            thirstyLavel = String.IsNullOrEmpty(thirstyLavel) ? "-1" : thirstyLavel;

            var filterModel = new PetFilterModelDTO()
            {
                HungerLavel = int.Parse(hungerLavel),
                ThirstyLavel = int.Parse(thirstyLavel),
                GameYear = 0,
                Age = 0,
                FeedingPeriod = 0,
                DrinkingPeriod = 0,
                IsLastHungerStage = false,
                IsLastThirstyStage = false
            };

            if (!String.IsNullOrEmpty(age))
            {
                filterModel.GameYear = _petInfoService.DayHours.Ticks * 365;
                filterModel.Age = filterModel.GameYear * int.Parse(age);
            }

            if (!String.IsNullOrEmpty(hungerLavel) && int.Parse(hungerLavel) != -1)
            {
                filterModel.FeedingPeriod = _petInfoService.FeedingPeriodHours.Ticks;
                if ((int)HungerLavel.Dead == filterModel.HungerLavel)
                    filterModel.IsLastHungerStage = true;
            }

            if (!String.IsNullOrEmpty(thirstyLavel) && int.Parse(thirstyLavel) != -1)
            {
                filterModel.DrinkingPeriod = _petInfoService.DrinkingPeriodHours.Ticks;
                if ((int)ThirstyLavel.Dead == filterModel.ThirstyLavel)
                    filterModel.IsLastThirstyStage = true;
            }

            PaginatedListDTO<PetDTO>? pets = await GetPage(page, sortType, filterModel);
            if (pets == null)
                return RedirectToAction("Login", "Users");

            var vm = new AllPetsViewModel
            {
                PaginatedList = pets,
                SortModel = new PetSortModel(sortType),
                FilterModel = new FilterPetViewModel(age, filterModel.HungerLavel, filterModel.ThirstyLavel)
            };
            return View(vm);
        }

        public async Task<IEnumerable<PetDTO>?> GetAllPets()
        {
            return await _petService.GetAll();
        }

        public async Task<IActionResult> Create(string name, string appearance)
        {
            var result = await _petService.Create(name, appearance, GetAuthorizedUserFarmId());
            if (result != -1)
            {
                return RedirectToAction("UserFarm", "Farms", new { id = GetAuthorizedUserFarmId() });
            }
            else
                return BadRequest();
        }

        public async Task<IActionResult> Feed(int id)
        {
            if (await _petService.Feed(id))
            {
                return RedirectToAction("GetCurrentUserFarm", "Farms");
            }
            else
                return BadRequest();
        }

        private async Task<bool> IsDeath(PetDTO pet)
        {
            long deathTime = pet.LastDrinkingTime.Ticks + _petInfoService.DrinkingPeriodHours.Ticks * (int)ThirstyLavel.Dead;
            if (DateTime.UtcNow.Ticks >= deathTime)
                return await Death(pet.Id, deathTime);
            deathTime = pet.LastFeedingTime.Ticks + _petInfoService.FeedingPeriodHours.Ticks * (int)HungerLavel.Dead;
            if (DateTime.UtcNow.Ticks >= deathTime)
                return await Death(pet.Id, deathTime);

            return false;
        }

        public async Task<bool> Death(int id, long deathTime)
        {
            return await _petService.Death(id, deathTime);
        }

        public async Task<IActionResult> Drink(int id)
        {
            if (await _petService.Drink(id))
            {
                return RedirectToAction("GetCurrentUserFarm", "Farms");
            }
            else
                return BadRequest();
        }
    }
}
