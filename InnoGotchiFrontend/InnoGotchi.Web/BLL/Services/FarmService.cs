using AutoMapper;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.BLL.DTO;

namespace InnoGotchi.Web.BLL.Services
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
