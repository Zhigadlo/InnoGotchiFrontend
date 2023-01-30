using AutoMapper;
using Hanssens.Net;
using InnoGotchi.BLL.DTO;
using InnoGotchi.DAL.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.BLL.Services
{
    public class ImageService : BaseService
    {
        private PictureManager _pictureManager;
        public ImageService(IMapper mapper,
                            IHttpClientFactory httpClientFactory,
                            LocalStorage localStorage,
                            IConfiguration configuration) : base(mapper)
        {
            _pictureManager = new PictureManager(httpClientFactory, localStorage, configuration);
        }

        public async Task<IEnumerable<PictureDTO>> GetAll()
        {
            var pictures = await _pictureManager.GetAll();
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
