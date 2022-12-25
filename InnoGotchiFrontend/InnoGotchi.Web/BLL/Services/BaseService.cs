using AutoMapper;

namespace InnoGotchi.Web.BLL.Services
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
