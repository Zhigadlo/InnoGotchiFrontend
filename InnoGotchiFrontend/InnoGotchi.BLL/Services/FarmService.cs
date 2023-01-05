using AutoMapper;
using InnoGotchi.BLL.DTO;
using InnoGotchi.DAL.Models;

namespace InnoGotchi.BLL.Services
{
    public class FarmService : BaseService
    {
        public FarmService(IMapper mapper) : base(mapper)
        {
        }

        public FarmDTO? GetFarmDTO(Farm? farm)
        {
            return _mapper.Map<FarmDTO>(farm);
        }
    }
}
