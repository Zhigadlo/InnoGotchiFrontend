using AutoMapper;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.BLL.DTO;

namespace InnoGotchi.Web.BLL.Services
{
    public class RequestService : BaseService
    {
        public RequestService(IMapper mapper) : base(mapper)
        {
        }

        public bool IsExist(int ownerId, int receipientId, IEnumerable<ColoborationRequestDTO> requests)
        {
            ColoborationRequestDTO? request = requests.FirstOrDefault(r => r.RequestOwnerId == ownerId
                                                                        && r.RequestReceipientId == receipientId);
            return requests.Contains(request);
        }

        public ColoborationRequestDTO? GetColoborationRequestDTO(ColoborationRequest request)
        {
            return _mapper.Map<ColoborationRequestDTO>(request);
        }

        public IEnumerable<ColoborationRequestDTO>? GetColoborationRequestsDTO(IEnumerable<ColoborationRequest> requests)
        {
            return _mapper.Map<IEnumerable<ColoborationRequestDTO>>(requests);
        }
    }
}
