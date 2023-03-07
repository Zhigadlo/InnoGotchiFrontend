using AutoMapper;
using Hanssens.Net;
using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Models;
using InnoGotchi.DAL.Managers;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.BLL.Services
{
    /// <summary>
    /// Class that have ability to get pets data from data access layer
    /// </summary>
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
        /// <summary>
        /// Gets pet by id
        /// </summary>
        public async Task<PetDTO?> Get(int id)
        {
            var pet = await _petManager.Get(id);
            var petDTO = _mapper.Map<PetDTO>(pet);
            return _petInfoService.FillPetDTO(petDTO);
        }
        /// <summary>
        /// Gets all pets
        /// </summary>
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
        /// <summary>
        /// Gets all pet names
        /// </summary>
        public async Task<IEnumerable<string>?> GetAllNames()
        {
            return await _petManager.GetAllNames();
        }
        /// <summary>
        /// Gets page by number with pets. This page was sorted and filtrated.
        /// </summary>
        public async Task<PaginatedListDTO<List<PetDTO>>?> GetPage(int page, string sortType, PetFilterModelDTO filterModel)
        {
            var pets = await _petManager.GetPage(page, sortType, _mapper.Map<PetFilterModel>(filterModel));

            if (pets == null)
                return null;

            PaginatedListDTO<List<PetDTO>>? result = new PaginatedListDTO<List<PetDTO>>
            {
                TotalPages = pets.TotalPages,
                PageIndex = pets.PageIndex,
                Items = _mapper.Map<List<PetDTO>>(pets.Items)
            };

            result.Items.ForEach(async p =>
            {
                if (_petInfoService.IsDeath(p))
                    Death(p.Id, p.DeadTime.Ticks);

                _petInfoService.FillPetDTO(p);
            });

            return result;
        }
        /// <summary>
        /// Creates new pet
        /// </summary>
        public async Task<int> Create(string name, string appearance, int farmId)
        {
            return await _petManager.Create(name, appearance, farmId);
        }
        /// <summary>
        /// Feeds pet by id
        /// </summary>
        public async Task<bool> Feed(int id)
        {
            return await _petManager.Feed(id);
        }
        /// <summary>
        /// Gives a drink to pet by id
        /// </summary>
        public async Task<bool> Drink(int id)
        {
            return await _petManager.Drink(id);
        }
        /// <summary>
        /// Sets dead status to pet
        /// </summary>
        public async Task<bool> Death(int id, long deathTime)
        {
            return await _petManager.Death(id, deathTime);
        }
    }
}
