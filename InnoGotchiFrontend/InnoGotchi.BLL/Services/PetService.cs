using AutoMapper;
using Hanssens.Net;
using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Models;
using InnoGotchi.DAL.Managers;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.BLL.Services
{
    public class PetService : BaseService
    {
        private PetInfoService _petInfoService;
        private PetManager _petManager;
        public PetService(IMapper mapper,
                          IConfiguration configuration,
                          IHttpClientFactory httpClientFactory,
                          LocalStorage localStorage) : base(mapper)
        {
            _petInfoService = new PetInfoService(configuration);
            _petManager = new PetManager(httpClientFactory, localStorage, configuration);
        }

        public async Task<PetDTO?> Get(int id)
        {
            var pet = await _petManager.Get(id);
            var petDTO = _mapper.Map<PetDTO>(pet);
            return _petInfoService.FillPetDTO(petDTO);
        }

        public async Task<IEnumerable<PetDTO>?> GetAll()
        {
            var pets = await _petManager.GetAll();
            if (pets == null)
                return null;

            var result = _mapper.Map<IEnumerable<PetDTO>>(pets);

            foreach (var pet in result)
            {
                var petDTO = _mapper.Map<PetDTO>(pet);
                _petInfoService.FillPetDTO(petDTO);
            }
            return result;
        }

        public async Task<IEnumerable<string>?> GetAllNames()
        {
            return await _petManager.GetAllNames();
        }

        public async Task<PaginatedListDTO<PetDTO>?> GetPage(int page, string sortType, PetFilterModelDTO filterModel)
        {
            var pets = await _petManager.GetPage(page, sortType, _mapper.Map<PetFilterModel>(filterModel));

            if (pets == null)
                return null;

            PaginatedListDTO<PetDTO>? result = new PaginatedListDTO<PetDTO>
            {
                TotalPages = pets.TotalPages,
                PageIndex = pets.PageIndex,
                Items = _mapper.Map<List<PetDTO>>(pets.Items)
            };

            return result;
        }

        public async Task<int> Create(string name, string appearance, int farmId)
        {
            return await _petManager.Create(name, appearance, farmId);
        }

        public async Task<bool> Feed(int id)
        {
            return await _petManager.Feed(id);
        }

        public async Task<bool> Drink(int id)
        {
            return await _petManager.Drink(id);
        }

        public async Task<bool> Death(int id, long deathTime)
        {
            return await _petManager.Death(id, deathTime);
        }
    }
}
