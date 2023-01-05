using AutoMapper;
using InnoGotchi.BLL.DTO;
using InnoGotchi.DAL.Models;

namespace InnoGotchi.BLL.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Pet, PetDTO>().ReverseMap();
            CreateMap<Farm, FarmDTO>().ReverseMap();
            CreateMap<Appearance, AppearanceDTO>().ReverseMap();
            CreateMap<ColoborationRequest, ColoborationRequestDTO>().ReverseMap();
        }
    }
}
