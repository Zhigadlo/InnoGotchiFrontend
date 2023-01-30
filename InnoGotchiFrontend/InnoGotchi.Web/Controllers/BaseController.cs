using InnoGotchi.BLL.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InnoGotchi.Web.Controllers
{
    /// <summary>
    /// Controller that have methods for getting info about authorized user
    /// </summary>
    public class BaseController : Controller
    {
        protected int GetAuthorizedUserId()
        {
            var id = HttpContext.User.FindFirstValue(nameof(SecurityToken.UserId));
            if (id == null)
                return -1;
            else
                return int.Parse(id);
        }

        protected int GetAuthorizedUserFarmId()
        {
            string? farmId = HttpContext.User.FindFirstValue(nameof(SecurityToken.FarmId));
            if (farmId == null)
                return -1;
            else
                return int.Parse(farmId);
        }
    }
}
