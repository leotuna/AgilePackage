using AgilePackage.Core.Services;
using AgilePackage.Web.App.Extensions;
using AgilePackage.Web.App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Web;

namespace AgilePackage.Web.App.Controllers
{
    [Route("/recover-password")]
    public class RecoverPasswordController : AgilePackageBaseController
    {
        private UserManager<User> UserManager { get; }
        private EmailService EmailService { get; }
        private IConfiguration Configuration { get; }

        public RecoverPasswordController(UserManager<User> userManager, EmailService emailService, IConfiguration configuration)
        {
            UserManager = userManager;
            EmailService = emailService;
            Configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(RecoverPasswordDto model)
        {
            var user = await UserManager.FindByNameAsync(model.Email);

            if (user is null)
            {
                this.ToastError("Email not found.");
                return View();
            }

            var code = await UserManager.GeneratePasswordResetTokenAsync(user);

            try
            {
                Uri.TryCreate($"{Configuration["Urls:App"]}/recover-password/confirm?id={HttpUtility.UrlEncode(code)}&email={user.Email}", UriKind.Absolute, out Uri uri);

                EmailService.Send(
                    to: user.Email,
                    subject: "Password recovery",
                    body: $"<p>To recover your password, please <a href='{uri}'>click here</a>.</p>",
                    isBodyHtml: true);

                return RedirectToAction(nameof(Sent));
            }
            catch
            {
                this.ToastError("Error to send you the recovery email.");
                return View();
            }
        }

        [HttpGet("sent")]
        [AllowAnonymous]
        public IActionResult Sent()
        {
            return View();
        }

        [HttpGet("/recover-password/confirm")]
        [AllowAnonymous]
        public IActionResult Confirm([FromQuery] string id, [FromQuery] string email)
        {
            return View(new ConfirmPasswordRecoveryDto { Id = id, Email = email });
        }

        [HttpPost("/recover-password/confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> Confirm([FromQuery] string id, [FromQuery] string email, ConfirmPasswordRecoveryDto model)
        {
            var user = await UserManager.FindByNameAsync(email);

            var result = await UserManager.ResetPasswordAsync(user, id, model.Password);

            if (!result.Succeeded)
            {
                this.ToastError("Error to recover password.");
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(SignIn), typeof(AuthController).ControllerName());
        }
    }
}
