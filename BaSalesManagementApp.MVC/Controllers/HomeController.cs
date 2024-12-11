using BaSalesManagementApp.Business.Interfaces;
using BaSalesManagementApp.Business.Services;
using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.DataAccess.Interfaces.Repositories;
using BaSalesManagementApp.MVC.Models;
using BaSalesManagementApp.MVC.Models.AppUserVMs;
using BaSalesManagementApp.MVC.Models.LoginVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Security.Claims;

namespace BaSalesManagementApp.MVC.Controllers
{
	//[Authorize(Roles = "admin")]
	public class HomeController : BaseController
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly IAdminRepository _adminRepository;
		private readonly IAccountService _accountService;
		private readonly IStringLocalizer<Resource> _stringLocalizer;
		public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IAdminRepository adminRepository, IAccountService accountService, IStringLocalizer<Resource> stringLocalizer)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			_adminRepository = adminRepository;
			_accountService = accountService;
			_stringLocalizer = stringLocalizer;
		}


		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginVM vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}
			var appUser = await _accountService.FindByEmailAsync(vm.Email);

			if (appUser is null)
			{
				NotifyError(_stringLocalizer[Messages.ACCOUNT_NOT_FOUND]);
				return View(vm);
			}

			// Kullanıcı ID'sini claim'lere ekle sonra order oluştururken kullanmak için
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, appUser.Id),
				new Claim(ClaimTypes.Name, appUser.UserName),
				new Claim(ClaimTypes.Email, appUser.Email)
			};

			Microsoft.AspNetCore.Identity.SignInResult signInResult = await _accountService.SignInAsync(appUser, vm.Password, false);
			if (!signInResult.Succeeded)
			{
				NotifyError(_stringLocalizer[Messages.ACCOUNT_NOT_FOUND]);
				return View(vm);
			}

			var roles = await _accountService.GetRolesAsync(appUser);
			if (roles is null)
			{
				NotifyError(_stringLocalizer[Messages.ACCOUNT_ROLE_NOT_FOUND_FOR_USER]);
				//NotifyError(_localizer[Messages.ACCOUNT_ROLE_NOT_FOUND_FOR_USER]);
				return View(vm);
			}

			if (appUser.EmailConfirmed == false)
			{
				// Bir token üreten bölüm
				var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
				// token ile birlikte Accountcontroller'ın ChangePassword Action'una giden bölüm
				return RedirectToAction("ChangePassword", "Account", new { token });
			}

			return RedirectToAction("Index", "Home");

		}

		public async Task<IActionResult> LogOut()
		{
			await _accountService.SignOutAsync();
			return RedirectToAction("Login", "Home");
		}

		public IActionResult Index()
		{

			return View();
		}
		//Language
		public IActionResult ChangeLanguage(string culture)
		{
			var cookieValue = $"c={culture}|uic={culture}";
			Response.Cookies.Append(".AspNetCore.Culture", cookieValue, new CookieOptions
			{
				Expires = DateTimeOffset.UtcNow.AddYears(1),
				Path = "/"
			});
			return Redirect(Request.Headers["Referer"].ToString());

		}
		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
