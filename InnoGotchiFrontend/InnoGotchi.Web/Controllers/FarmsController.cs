using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Services;
using InnoGotchi.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.Web.Controllers
{
    public class FarmsController : BaseController
    {
        private FarmService _farmService;
        public FarmsController(FarmService farmService)
        {
            _farmService = farmService;
        }
        /// <summary>
        /// Gets farm by user id and goes to UserFarm view
        /// </summary>
        public async Task<IActionResult> UserFarm(int id)
        {
            int authorizedUserId = GetAuthorizedUserId();
            if (authorizedUserId == -1)
                return RedirectToAction("Login", "Users");

            var userFarmModel = new UserFarmModel
            {
                FarmNames = await _farmService.GetAllNames(),
                AuthorizedUserId = authorizedUserId,
                Farm = new FarmDTO { Id = id }
            };

            if (id == -1)
                return View("UserFarm", userFarmModel);
            FarmDTO? farm = await _farmService.Get(id);
            if (farm == null)
                return RedirectToAction("Login", "Users");

            userFarmModel.Farm = farm;
            return View("UserFarm", userFarmModel);
        }
        /// <summary>
        /// Creates farm and if creation was successful goes to UserFarm view
        /// </summary>
        public async Task<IActionResult> CreateFarm(string name)
        {
            var userId = GetAuthorizedUserId();
            if (userId == -1)
                return RedirectToAction("Login", "Users");
            var farmId = await _farmService.Create(userId, name);

            if (farmId != -1)
                return await UserFarm(farmId);
            else
                return BadRequest();
        }
        /// <summary>
        /// Gets authorized user farm and goes to UserFarm view
        /// </summary>
        public async Task<IActionResult> GetCurrentUserFarm()
        {
            var farmId = GetAuthorizedUserFarmId();
            return await UserFarm(farmId);
        }
        /// <summary>
        /// Gets all farm names that already in use
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>?> GetAllFarmNames()
        {
            return await _farmService.GetAllNames();
        }
    }
}
