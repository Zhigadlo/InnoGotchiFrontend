using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Services;
using InnoGotchi.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.Web.Controllers
{
    public class PicturesController : BaseController
    {
        private ImageService _imageService;
        private PetService _petService;
        private PetInfoService _petInfoService;
        public PicturesController(ImageService imageService,
                                  PetService petService,
                                  PetInfoService petInfoService)
        {
            _petInfoService = petInfoService;
            _imageService = imageService;
            _petService = petService;
        }
        /// <summary>
        /// Gets all body parts and goes to PetConstructor view
        /// </summary>
        public async Task<IActionResult> PetConstructor()
        {
            var petsController = new PetsController(_petService, _petInfoService);
            IEnumerable<string> petNames = await _petService.GetAllNames();
            IEnumerable<PictureDTO>? allPictures = await GetAll();
            PetConstructorViewModel vm = new PetConstructorViewModel
            {
                Bodies = allPictures.Where(p => p.Description.ToLower() == "body").ToList(),
                Eyes = allPictures.Where(p => p.Description.ToLower() == "eyes").ToList(),
                Noses = allPictures.Where(p => p.Description.ToLower() == "nose").ToList(),
                Mouths = allPictures.Where(p => p.Description.ToLower() == "mouth").ToList(),
                PetNames = petNames
            };
            return View(vm);
        }
        /// <summary>
        /// Goes to view with all pet body parts
        /// </summary>
        /// <returns></returns>
        public IActionResult AllBodyParts()
        {
            return View(GetAll());
        }

        /// <summary>
        /// Gets all pictures
        /// </summary>
        public async Task<IEnumerable<PictureDTO>?> GetAll()
        {
            return await _imageService.GetAll();
        }
    }
}
