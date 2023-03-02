using AutoMapper;
using Hanssens.Net;
using InnoGotchi.BLL.DTO;
using InnoGotchi.DAL.Managers;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.BLL.Services
{
    /// <summary>
    /// Class that have ability to get farms data from data access layer
    /// </summary>
    public class FarmService : BaseService
    {
        private PetInfoService _petInfoService;
        private FarmManager _farmManager;
        private PetManager _petManager;
        public FarmService(IMapper mapper,
                           IConfiguration configuration,
                           IHttpClientFactory httpClientFactory,
                           LocalStorage localStorage) : base(mapper)
        {
            _petManager = new PetManager(httpClientFactory, localStorage, configuration);
            _petInfoService = new PetInfoService(configuration);
            _farmManager = new FarmManager(httpClientFactory, localStorage, configuration);
        }
        /// <summary>
        /// Gets farm by id
        /// </summary>
        public async Task<FarmDTO?> Get(int id)
        {
            var farm = await _farmManager.Get(id);
            if (farm == null)
                return null;
            var farmDTO = _mapper.Map<FarmDTO>(farm);
            farmDTO.Pets.ForEach(async p =>
            {
                if (_petInfoService.IsDeath(p))
                    _petManager.Death(p.Id, p.DeadTime.Ticks);
                _petInfoService.FillPetDTO(p);
            });
            return farmDTO;
        }
        /// <summary>
        /// Gets all farms
        /// </summary>
        public async Task<IEnumerable<Farm>?> GetAll()
        {
            return await _farmManager.GetAll();
        }
        /// <summary>
        /// Gets all farm names
        /// </summary>
        public async Task<IEnumerable<string>?> GetAllNames()
        {
            return await _farmManager.GetAllNames();
        }
        /// <summary>
        /// Creates new farm
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<int> Create(int ownerId, string name)
        {
            return await _farmManager.Create(ownerId, name);
        }
    }
}
