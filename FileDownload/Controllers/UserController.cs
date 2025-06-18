using FileDownload.DTO;
using FileDownload.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FileDownload.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserServiceInterface _userService;
        public UserController(IUserServiceInterface userService)
        {
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserDto registerUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (registerUser == null) return BadRequest(new { Message = "Invalid registeration request" });

                    var result = await _userService.RegisterUserAsync(registerUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Login));
                    }
                    return BadRequest(new { Message = "Unexpected Error! Failed to register user", Error = result.Errors.Select(e => e.Description) });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "An error occured when registring user", ex });
                }
            }
            else
            {
                return View(registerUser);
            } 
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto loginUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (loginUser == null) return BadRequest(new { Message = "Invalid Login Request" });
                    var result = await _userService.LoginUserAsync(loginUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(controllerName: "Home", actionName: "Index");
                    }
                    return BadRequest(new { Message = "Unexpected Error! Failed to login user", Error = result.Errors.Select(e => e.Description) });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "An error occured when logging", Error = ex.Message });
                }
            }
            else
            {
                return View(loginUser);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var result = await _userService.LogoutAsync();
                if (result.Succeeded)
                {
                    return RedirectToAction(controllerName: "home", actionName: nameof(Index));
                }
                return BadRequest(new { Message = "Unexpected Error! Failed to loggingout user", Error = result.Errors.Select(e => e.Description) });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "An error occured when logging out", Error = ex.Message });
            }

        }
    }
}
