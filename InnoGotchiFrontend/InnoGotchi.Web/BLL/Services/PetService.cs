using AutoMapper;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.BLL.DTO;

namespace InnoGotchi.Web.BLL.Services
{
    public class PetService : BaseService
    {
        public PetService(IMapper mapper) : base(mapper)
        {
        }

        public PetDTO Get(Pet? pet)
        {
            return _mapper.Map<PetDTO>(pet);
        }

        public IEnumerable<PetDTO>? GetAll(IEnumerable<Pet>? pets)
        {
            return _mapper.Map<IEnumerable<PetDTO>>(pets);
        }
    }
}
