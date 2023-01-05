using AutoMapper;
using InnoGotchi.BLL.DTO;
using InnoGotchi.DAL.Models;

namespace InnoGotchi.BLL.Services
{
    public class UserService : BaseService
    {
        public UserService(IMapper mapper) : base(mapper)
        {
        }

        public UserDTO? GetUserDTO(User? user)
        {
            return _mapper.Map<UserDTO>(user);
        }

        public IEnumerable<UserDTO>? GetAll(IEnumerable<User>? users)
        {
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public User GetUser(UserDTO user)
        {
            return _mapper.Map<User>(user);
        }
    }
}
