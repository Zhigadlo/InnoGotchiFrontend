using AutoMapper;
using InnoGotchi.BLL.DTO;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.BLL.Services
{
    public class FarmService : BaseService
    {
        private PetInfoService _petInfoService;
        public FarmService(IMapper mapper, IConfiguration configuration) : base(mapper)
        {
            _petInfoService = new PetInfoService(configuration);
        }

        public FarmDTO? GetFarmDTO(Farm? farm)
        {
            var farmDTO = _mapper.Map<FarmDTO>(farm);
            farmDTO.Pets.ForEach(p => p = _petInfoService.FillPetDTO(p));
            return farmDTO;
        }
    }
}
