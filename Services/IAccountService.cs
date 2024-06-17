using Microsoft.AspNetCore.Identity;

namespace ContactAppWeb.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
        Task<SignInResult> LoginUserAsync(LoginViewModel model);
        Task LogoutUserAsync();
    }
}
