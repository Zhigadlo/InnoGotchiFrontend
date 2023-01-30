using InnoGotchi.BLL.DTO;
using InnoGotchi.BLL.Identity;
using InnoGotchi.BLL.Services;
using InnoGotchi.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InnoGotchi.Web.Controllers
{
    public class UsersController : BaseController
    {
        private UserService _userService;
        private ImageService _imageService;
        public UsersController(UserService userService,
                               ImageService imageService)
        {
            _userService = userService;
            _imageService = imageService;
        }

        public IActionResult Login(string errorMessage)
        {
            return View(errorMessage);
        }

        public async Task<IActionResult> Register()
        {
            IEnumerable<string> userEmails = await _userService.GetAllEmails();
            return View(userEmails);
        }

        public IActionResult ChangePasswordView()
        {
            return View();
        }

        public async Task<IActionResult> Coloborators()
        {
            int userId = GetAuthorizedUserId();
            if (userId == -1)
                RedirectToAction("Login");

            var coloborators = await _userService.Coloborators(userId);
            return View(coloborators);
        }

        public async Task<IActionResult> UserRequests()
        {
            UserDTO? user = await GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login");
            List<KeyValuePair<int, UserDTO>> usersWhoSentRequest = new List<KeyValuePair<int, UserDTO>>();
            foreach (var rr in user.ReceivedRequests)
            {
                if (rr.IsConfirmed == false)
                {
                    var u = await Get(rr.RequestOwnerId);

                    usersWhoSentRequest.Add(KeyValuePair.Create(rr.Id, u));
                }
            };
            List<KeyValuePair<int, UserDTO>> usersWhoReceivedRequest = new List<KeyValuePair<int, UserDTO>>();
            foreach (var sr in user.SentRequests)
            {
                if (sr.IsConfirmed == false)
                {
                    var u = await Get(sr.RequestReceipientId);
                    usersWhoReceivedRequest.Add(KeyValuePair.Create(sr.Id, u));
                }
            };
            UserRequestsViewModel vm = new UserRequestsViewModel()
            {
                UsersWhoSentRequest = usersWhoSentRequest,
                UsersWhoReceivedRequest = usersWhoReceivedRequest
            };
            return View(vm);
        }

        public async Task<IActionResult> AllUsers()
        {
            IEnumerable<UserDTO>? usersEnumerable = await _userService.GetAll();
            if (usersEnumerable == null)
                return RedirectToAction("Login");
            var users = usersEnumerable.ToList();
            int authorized_id = GetAuthorizedUserId();
            if (authorized_id == -1)
                return RedirectToAction("Login");
            UserDTO authorizedUser = users.ToList().Find(u => u.Id == authorized_id);
            users.Remove(authorizedUser);
            List<UserViewModel> usersVM = new List<UserViewModel>();
            foreach (UserDTO user in users)
            {
                UserViewModel userVM = new UserViewModel
                {
                    User = user
                };

                var sentRequest = authorizedUser.SentRequests.FirstOrDefault(sr => sr.RequestReceipientId == user.Id);
                var receivedRequest = authorizedUser.ReceivedRequests.FirstOrDefault(sr => sr.RequestOwnerId == user.Id);

                if (sentRequest != null)
                {
                    if (sentRequest.IsConfirmed)
                        userVM.RequestState = RequestState.Confirmed;
                    else
                        userVM.RequestState = RequestState.Sent;

                    userVM.RequestId = sentRequest.Id;
                }
                else if (receivedRequest != null)
                {
                    if (receivedRequest.IsConfirmed)
                        userVM.RequestState = RequestState.Confirmed;
                    else
                        userVM.RequestState = RequestState.Received;

                    userVM.RequestId = receivedRequest.Id;
                }
                else
                    userVM.RequestState = RequestState.NotUsed;

                usersVM.Add(userVM);
            }
            return View(usersVM);
        }
        public async Task<IActionResult> ChangeAvatar(IFormFile FormFile)
        {
            bool result = await AvatarUpdate(FormFile);
            if (result)
                return RedirectToAction("UserProfile");
            else
                return BadRequest();
        }

        public async Task<IActionResult> UserProfile()
        {
            UserDTO? user = await GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login");
            return View(user);
        }

        private async Task<UserDTO?> GetCurrentUser()
        {
            int userId = GetAuthorizedUserId();
            if (userId == -1)
            {
                return null;
            }
            return await Get(userId);
        }
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userId = GetAuthorizedUserId();
            if (userId == -1)
                return RedirectToAction("Login");
            if (await _userService.ChangePassword(userId, oldPassword, newPassword, confirmPassword))
            {
                return await Logout();
            }
            else
                return View("ChangePasswordView", new ErrorModel { Error = "Old password is wrong" });
        }
        public async Task<IActionResult> Logout()
        {
            _userService.RemoveTokenFromLocalStorage();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Authenticate(string email, string password)
        {
            var token = await _userService.Authenticate(email, password);
            if (token != null)
            {
                await SignIn(token);
                return RedirectToAction("Index", "Home");
            }
            else
                return View("Login", new ErrorModel { Error = "Invalid email or password" });
        }

        private async Task SignIn(SecurityToken securityToken)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, securityToken.Email),
                new Claim(ClaimTypes.Name, securityToken.UserName),
                new Claim(nameof(SecurityToken.AccessToken), securityToken.AccessToken),
                new Claim(nameof(SecurityToken.ExpireAt), securityToken.ExpireAt.ToString()),
                new Claim(nameof(SecurityToken.UserId), securityToken.UserId.ToString()),
                new Claim(nameof(SecurityToken.FarmId), securityToken.FarmId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(claimsPrincipal);
        }

        public async Task<UserDTO?> Get(int id)
        {
            return await _userService.Get(id);
        }
        public async Task<IEnumerable<UserDTO>?> GetAll()
        {
            return await _userService.GetAll();
        }

        public async Task<IActionResult> Create(UserDTO? user)
        {
            user.Avatar = _imageService.GetBytesFromFormFile(user.FormFile);
            var result = await _userService.Create(user);
            if (result != -1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
                return BadRequest();
        }

        public async Task<bool> AvatarUpdate(IFormFile FormFile)
        {
            var userId = GetAuthorizedUserId();
            var appearance = _imageService.GetBytesFromFormFile(FormFile);

            return await _userService.AvatarUpdate(userId, appearance);
        }
    }
}
