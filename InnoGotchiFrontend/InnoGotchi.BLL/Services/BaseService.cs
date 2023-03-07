using AutoMapper;

namespace InnoGotchi.BLL.Services
{
    /// <summary>
    /// Class that contains mapper
    /// </summary>
    public class BaseService
    {
        protected IMapper _mapper;
        public BaseService(IMapper mapper)
        {
            _mapper = mapper;
        }

    }
}
