using AutoMapper;
using InnoGotchi.BLL.DTO;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace InnoGotchi.BLL.Services
{
    public class PetService : BaseService
    {
        private PetInfoService _petInfoService;
        public PetService(IMapper mapper, IConfiguration configuration) : base(mapper)
        {
            _petInfoService = new PetInfoService(configuration);
        }

        public PetDTO Get(Pet? pet)
        {
            if (pet == null)
                return null;

            var petDTO = _mapper.Map<PetDTO>(pet);

            return _petInfoService.FillPetDTO(petDTO);
        }

        public IEnumerable<PetDTO>? GetAll(IEnumerable<Pet>? pets)
        {
            if (pets == null)
                return null;

            List<PetDTO>? result = new List<PetDTO>();
            foreach (var pet in pets)
            {
                var petDTO = _mapper.Map<PetDTO>(pet);
                result.Add(_petInfoService.FillPetDTO(petDTO));
                
            }
            return result.AsEnumerable();
        }
    }
}
