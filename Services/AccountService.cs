using ContactAppWeb.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ContactAppWeb.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task<SignInResult> LoginUserAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        }

        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
