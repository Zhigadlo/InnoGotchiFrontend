using AutoMapper;
using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Models;
using InnoGotchi.DAL.Models;
using Microsoft.Extensions.Configuration;

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

        public PaginatedList<PetDTO> GetPage(PaginatedList<Pet>? pets)
        {
            if (pets == null)
                return null;

            PaginatedList<PetDTO>? result = new PaginatedList<PetDTO>
            {
                TotalPages = pets.TotalPages,
                PageIndex = pets.PageIndex,
                Items = new List<PetDTO>()
            };

            foreach (var pet in pets.Items)
            {
                var petDTO = _mapper.Map<PetDTO>(pet);
                result.Items.Add(_petInfoService.FillPetDTO(petDTO));
            }

            return result;
        }
    }
}
