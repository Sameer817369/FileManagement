using FileDownload.DTO;
using Microsoft.AspNetCore.Identity;

namespace FileDownload.Service.Interface
{
    public interface IUserServiceInterface
    {
        Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerUser);
        Task<IdentityResult> LoginUserAsync(LoginUserDto loginUser);
        Task<IdentityResult> LogoutAsync();
    }
}
