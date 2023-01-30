using AutoMapper;
using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Identity;
using InnoGotchi.BLL.Models;
using InnoGotchi.DAL.Identity;
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
            CreateMap<Picture, PictureDTO>().ReverseMap();
            CreateMap<ColoborationRequest, ColoborationRequestDTO>().ReverseMap();
            CreateMap<PetFilterModel, PetFilterModelDTO>().ReverseMap();
            CreateMap<SecurityTokenModel, SecurityToken>().ReverseMap();
        }
    }
}
