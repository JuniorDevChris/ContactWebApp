using ContactAppWeb.Models;
using ContactAppWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ContactAppWeb.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["HideSearchBar"] = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.RegisterUserAsync(model);
                if (result.Succeeded)
                {
                    await _accountService.LoginUserAsync(new LoginViewModel { Email = model.Email, Password = model.Password });
                    return RedirectToAction("Index", "Contact");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewData["HideSearchBar"] = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.LoginUserAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Contact");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutUserAsync();
            return RedirectToAction("Index", "Contact");
        }
    }
}