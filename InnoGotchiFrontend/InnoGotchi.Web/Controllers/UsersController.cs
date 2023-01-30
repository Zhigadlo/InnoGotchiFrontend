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
        /// <summary>
        /// Goes to Login view
        /// </summary>
        public IActionResult Login(string errorMessage)
        {
            return View(errorMessage);
        }
        /// <summary>
        /// Goes to Register view
        /// </summary>
        public async Task<IActionResult> Register()
        {
            IEnumerable<string> userEmails = await _userService.GetAllEmails();
            return View(userEmails);
        }
        /// <summary>
        /// Goes to ChangePasswordView page
        /// </summary>
        public IActionResult ChangePasswordView()
        {
            return View();
        }
        /// <summary>
        /// Gets all coloborators and goes to coloborators view
        /// </summary>
        public async Task<IActionResult> Coloborators()
        {
            int userId = GetAuthorizedUserId();
            if (userId == -1)
                RedirectToAction("Login");

            var coloborators = await _userService.Coloborators(userId);
            return View(coloborators);
        }
        /// <summary>
        /// Gets all user requests and goes to UserRequests view
        /// </summary>
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
        /// <summary>
        /// Gets all users and goes to AllUsers view
        /// </summary>
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
        /// <summary>
        /// Updates user avatar and redirect to UserProfile view
        /// </summary>
        public async Task<IActionResult> ChangeAvatar(IFormFile FormFile)
        {
            bool result = await AvatarUpdate(FormFile);
            if (result)
                return RedirectToAction("UserProfile");
            else
                return BadRequest();
        }
        /// <summary>
        /// Goes to UserProfile view
        /// </summary>
        public async Task<IActionResult> UserProfile()
        {
            UserDTO? user = await GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login");
            return View(user);
        }
        /// <summary>
        /// Gets authorized user
        /// </summary>
        private async Task<UserDTO?> GetCurrentUser()
        {
            int userId = GetAuthorizedUserId();
            if (userId == -1)
            {
                return null;
            }
            return await Get(userId);
        }
        /// <summary>
        /// Updates user password
        /// </summary>
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
        /// <summary>
        /// Logout user
        /// </summary>
        public async Task<IActionResult> Logout()
        {
            _userService.RemoveTokenFromLocalStorage();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// AUuthenticates user
        /// </summary>
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
        /// <summary>
        /// User sign in
        /// </summary>
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
        /// <summary>
        /// Gets user by id
        /// </summary>
        public async Task<UserDTO?> Get(int id)
        {
            return await _userService.Get(id);
        }
        /// <summary>
        /// Gets all users
        /// </summary>
        public async Task<IEnumerable<UserDTO>?> GetAll()
        {
            return await _userService.GetAll();
        }
        /// <summary>
        /// Create new user
        /// </summary>
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
        /// <summary>
        /// Updates user avatar
        /// </summary>
        public async Task<bool> AvatarUpdate(IFormFile FormFile)
        {
            var userId = GetAuthorizedUserId();
            var appearance = _imageService.GetBytesFromFormFile(FormFile);

            return await _userService.AvatarUpdate(userId, appearance);
        }
    }
}
