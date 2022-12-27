using AutoMapper;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.BLL.DTO;

namespace InnoGotchi.Web.BLL.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Pet, PetDTO>().ReverseMap();
            CreateMap<Farm, FarmDTO>().ReverseMap();
            CreateMap<Appearance, AppearanceDTO>().ReverseMap();
            //CreateMap<ColoborationRequest, ColoborationRequestDTO>().ReverseMap();
        }
    }
}
