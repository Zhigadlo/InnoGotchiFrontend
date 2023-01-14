using AutoMapper;
using InnoGotchi.BLL.DTO;
using InnoGotchi.DAL.Models;
using Microsoft.AspNetCore.Http;

namespace InnoGotchi.BLL.Services
{
    public class ImageService : BaseService
    {
        public ImageService(IMapper mapper) : base(mapper)
        {
        }

        public PictureDTO GetPictureDTO(Picture picture)
        {
            return _mapper.Map<PictureDTO>(picture);
        }

        public IEnumerable<PictureDTO> GetAll(IEnumerable<Picture> pictures)
        {
            return _mapper.Map<IEnumerable<PictureDTO>>(pictures);
        }
        public byte[]? GetBytesFromFormFile(IFormFile fileForm)
        {
            if (fileForm != null)
            {
                byte[]? imageData = null;
                using (var binaryReader = new BinaryReader(fileForm.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)fileForm.Length);
                }
                return imageData;
            }
            else
                return null;
        }
    }
}
