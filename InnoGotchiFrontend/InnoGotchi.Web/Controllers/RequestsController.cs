using InnoGotchi.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.Web.Controllers
{
    public class RequestsController : BaseController
    {
        private RequestService _requestService;
        public RequestsController(RequestService requestService)
        {
            _requestService = requestService;
        }
        /// <summary>
        /// Sents coloboration request to other user
        /// </summary>
        public async Task<IActionResult> Create(int receiverId)
        {
            int ownerId = GetAuthorizedUserId();
            if (ownerId == -1)
                return RedirectToAction("Login", "Users");

            if (await _requestService.Create(ownerId, receiverId))
            {
                return RedirectToAction("AllUsers", "Users");
            }
            else
                return BadRequest();
        }
        /// <summary>
        /// Confirms coloboration request
        /// </summary>
        public async Task<IActionResult> Confirm(int requestId, string actionName, string controllerName)
        {
            if (await _requestService.Confirm(requestId))
            {
                return RedirectToAction(actionName, controllerName);
            }
            else
                return BadRequest();
        }
        /// <summary>
        /// Deletes coloboration request
        /// </summary>
        public async Task<IActionResult> Delete(int requestId, string actionName, string controllerName)
        {
            if (await _requestService.Delete(requestId))
            {
                return RedirectToAction(actionName, controllerName);
            }
            else
                return BadRequest();
        }
    }
}
