using InnoGotchi.BLL.Services;
using InnoGotchi.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InnoGotchi.Web.Controllers
{
    public class HomeController : Controller
    {
        private PetInfoService _petInfoService;
        public HomeController(PetInfoService petInfoService)
        {
            _petInfoService = petInfoService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View(_petInfoService);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}