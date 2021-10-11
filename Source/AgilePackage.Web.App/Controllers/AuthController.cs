using AgilePackage.Web.App.Extensions;
using AgilePackage.Web.App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AgilePackage.Web.App.Controllers
{
    public class AuthController : AgilePackageBaseController
    {
        private IConfiguration Configuration { get; }
        private SignInManager<User> SignInManager { get; }
        private UserManager<User> UserManager { get; }

        public AuthController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IConfiguration configuration)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            Configuration = configuration;
        }

        [HttpGet("/")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                //return RedirectToAction(nameof(SubscriptionController.Index), typeof(SubscriptionController).ControllerName());
                return RedirectToAction(nameof(ProjectController.Index), typeof(ProjectController).ControllerName());
            }
            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet("/sign-in")]
        [AllowAnonymous]
        public IActionResult SignIn([FromQuery] bool invite = false)
        {
            return View(new SignInDto { Invite = invite });
        }

        [HttpPost("/sign-in")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(SignInDto model)
        {
            var result = await SignInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                this.ToastError("Email and password do not match.");
                return View();
            }

            if (model.Invite)
            {
                return RedirectToAction(nameof(InviteController.Index), typeof(InviteController).ControllerName());
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/sign-up")]
        [AllowAnonymous]
        public IActionResult SignUp([FromQuery] bool invite = false)
        {
            if (User.Identity is null)
            {
                return View();
            }
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(new SignUpDto { Invite = invite });
        }

        [HttpPost("/sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(SignUpDto model)
        {
            var user = new User { Name = model.Name, UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                this.ToastError("Error to create your account.");
                return View();
            }
            await SignInManager.SignInAsync(user, isPersistent: false);
            if (model.Invite)
            {
                return RedirectToAction(nameof(InviteController.Index), typeof(InviteController).ControllerName());
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/log-out")]
        public async Task<IActionResult> LogOut()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }
    }
}
