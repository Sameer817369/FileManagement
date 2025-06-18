using FileDownload.DTO;
using FileDownload.Models;
using FileDownload.Service.Interface;
using Microsoft.AspNetCore.Identity;

namespace FileDownload.Service
{
    public class UserService : IUserServiceInterface
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerUser)
        {
            try
            {
                if (registerUser == null) throw new ArgumentNullException("User info not send");
                var userModel = new AppUser
                {
                    Name = registerUser.Name,
                    Email = registerUser.Email,
                    PhoneNumber = registerUser.PhoneNumber,
                    Address = registerUser.Address,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    UserName = registerUser.Username
                };
                var result = await _userManager.CreateAsync(userModel, registerUser.Password);
                if (result.Succeeded)
                {
                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError { Description = $"Failed to register user {result.Errors.Select(e=>e.Description)}"});
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Internal Server Error! Failed To Register User {ex.Message}");
            }
        }
        public async Task<IdentityResult> LoginUserAsync(LoginUserDto loginUser)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginUser.Email) ?? throw new NullReferenceException("Email not found");
                if (loginUser == null) throw new ArgumentNullException("Invalid Credentials");
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginUser.Password, false);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError { Description = "Incorrect password or email" });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Internal Server Error! Login Failed {ex.Message}");
            }
        }
        public async Task<IdentityResult> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return IdentityResult.Success;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Internal Server Error! Logout Failed {ex.Message}");
            }
        }

    
    }
}
