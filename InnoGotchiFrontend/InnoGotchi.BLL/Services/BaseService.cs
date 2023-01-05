using AutoMapper;

namespace InnoGotchi.BLL.Services
{
    public class BaseService
    {
        protected IMapper _mapper;
        public BaseService(IMapper mapper)
        {
            _mapper = mapper;
        }

    }
}
