using Hanssens.Net;
using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Identity;
using InnoGotchi.BLL.Models;
using InnoGotchi.BLL.Services;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace InnoGotchi.Web.Controllers
{
    public class PetsController : BaseController
    {
        private PetService _petService;
        private PetInfoService _petInfoService;
        public PetsController(IHttpClientFactory httpClientFactory,
                              PetService petService,
                              PetInfoService petInfoService,
                              LocalStorage localStorage) : base(httpClientFactory, localStorage)
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
            var httpClient = GetHttpClient("Pets");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + $"/{id}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                Pet? pet = await JsonSerializer.DeserializeAsync<Pet>(contentStream, options);

                await IsDeath(pet);
                return _petService.Get(pet);
            }
            else
                return null;
        }

        public async Task<PaginatedList<PetDTO>> GetPage(int page, string sortType, long age, long year,
                                    int hungerLavel, long feedingPeriod, bool isLastFeedingStage, int thirstyLavel, long drinkingPeriod, bool isLastDrinkingStage)
        {
            var httpClient = GetHttpClient("Pets");
            var path = $"/{page}&{sortType}&{age}&{year}&{hungerLavel}&{feedingPeriod}&{isLastFeedingStage}&{thirstyLavel}&{drinkingPeriod}&{isLastDrinkingStage}";

            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + path
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                PaginatedList<Pet>? pets = await JsonSerializer.DeserializeAsync<PaginatedList<Pet>>(contentStream, options);

                pets.Items.ForEach(async p => await IsDeath(p));

                return _petService.GetPage(pets);
            }
            else
                return null;
        }

        private string GeStringFromSession(string key, string queryName, string defaultValue = "")
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
            string age = GeStringFromSession("filterage", "age");
            HttpContext.Session.SetString("filterage", age);

            string hungerLavel = GeStringFromSession("filterhunger", "hungerLavel");
            HttpContext.Session.SetString("filterhunger", hungerLavel);
            hungerLavel = String.IsNullOrEmpty(hungerLavel) ? "-1" : hungerLavel;

            string thirstyLavel = GeStringFromSession("filterthirsty", "thirstyLavel");
            HttpContext.Session.SetString("filterthirsty", thirstyLavel);
            thirstyLavel = String.IsNullOrEmpty(thirstyLavel) ? "-1" : thirstyLavel;

            int hunger = int.Parse(hungerLavel);
            int thirsty = int.Parse(thirstyLavel);
            long year = 0, ageDateTime = 0, feedingPeriod = 0, drinkingPeriod = 0;
            bool isLastFeedingStage = false, isLastDrinkingStage = false;

            if (age != null && !String.IsNullOrEmpty(age))
            {
                year = _petInfoService.DayHours.Ticks * 365;
                ageDateTime = year * int.Parse(age);
            }

            if (hungerLavel != null && !String.IsNullOrEmpty(hungerLavel) && int.Parse(hungerLavel) != -1)
            {
                feedingPeriod = _petInfoService.FeedingPeriodHours.Ticks;
                if ((int)HungerLavel.Dead == int.Parse(hungerLavel))
                    isLastFeedingStage = true;
            }

            if (thirstyLavel != null && !String.IsNullOrEmpty(thirstyLavel) && int.Parse(thirstyLavel) != -1)
            {
                drinkingPeriod = _petInfoService.DrinkingPeriodHours.Ticks;
                if ((int)ThirstyLavel.Dead == int.Parse(thirstyLavel))
                    isLastDrinkingStage = true;
            }

            PaginatedList<PetDTO> pets = await GetPage(page, sortType, ageDateTime, year, hunger, feedingPeriod, isLastFeedingStage, thirsty, drinkingPeriod, isLastDrinkingStage);
            if (pets == null)
                return RedirectToAction("Login", "Users");
            var vm = new AllPetsViewModel
            {
                PaginatedList = pets,
                SortModel = new PetSortModel(sortType),
                FilterModel = new FilterPetModel(age, int.Parse(hungerLavel), int.Parse(thirstyLavel))
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IEnumerable<PetDTO>?> GetAllPets()
        {
            var httpClient = GetHttpClient("Pets");

            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                IEnumerable<Pet>? pets = await JsonSerializer.DeserializeAsync<IEnumerable<Pet>>(contentStream, options);
                foreach (var p in pets)
                {
                    await IsDeath(p);
                }
                return _petService.GetAll(pets.AsEnumerable());
            }
            else
                return null;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string appearance)
        {
            var httpClient = GetHttpClient("Pets");

            var parameters = new Dictionary<string, string>();
            parameters["Name"] = name;
            parameters["Appearance"] = appearance;
            parameters["FarmId"] = HttpContext.User.FindFirstValue(nameof(SecurityToken.FarmId));

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("UserFarm", "Farms", new { id = HttpContext.User.FindFirstValue(nameof(SecurityToken.FarmId)) });
            }
            else
                return BadRequest();
        }

        public async Task<IActionResult> Feed(int id)
        {
            var httpClient = GetHttpClient("Pets");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Put,
                httpClient.BaseAddress + $"/feed/{id}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("GetCurrentUserFarm", "Farms");
            }
            else
                return BadRequest();
        }

        private async Task<bool> IsDeath(Pet pet)
        {
            long deathTime = pet.LastDrinkingTime.Ticks + _petInfoService.DrinkingPeriodHours.Ticks * (int)ThirstyLavel.Dead;
            if (DateTime.UtcNow.Ticks <= deathTime)
                return await Death(pet.Id, deathTime);
            deathTime = pet.LastFeedingTime.Ticks + _petInfoService.FeedingPeriodHours.Ticks * (int)HungerLavel.Dead;
            if (DateTime.UtcNow.Ticks <= deathTime)
                return await Death(pet.Id, deathTime);

            return false;
        }

        public async Task<bool> Death(int id, long deathTime)
        {   
            var httpClient = GetHttpClient("Pets");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Put,
                httpClient.BaseAddress + $"/death/{id}&{deathTime}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            else
                return false;
        }

        public async Task<IActionResult> Drink(int id)
        {
            var httpClient = GetHttpClient("Pets");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Put,
                httpClient.BaseAddress + $"/drink/{id}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("GetCurrentUserFarm", "Farms");
            }
            else
                return BadRequest();
        }
    }
}
