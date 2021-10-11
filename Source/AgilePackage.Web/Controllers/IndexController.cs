using AgilePackage.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace AgilePackage.Web.Controllers
{
    public class IndexController : Controller
    {
        private IConfiguration Configuration { get; set; }

        public IndexController(
            IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/contact")]
        public IActionResult Contact([FromQuery] bool? success)
        {
            if (success.HasValue && success == true)
            {
                return View(new Contact { Success = true });
            }
            return View(new Contact());
        }

        [HttpPost("/contact")]
        public IActionResult Contact(Contact model)
        {
            if (!string.IsNullOrWhiteSpace(model.City))
            {
                return RedirectToAction(nameof(Contact), new { success = true });
            }

            var client = MakeClient();
            var from = new MailAddress("agilepackage@gmail.com", "Agile Package");
            var message = new MailMessage(from, from)
            {
                Subject = $"/contact - {model.Email}",
                Body = model.Message
            };

            try
            {
                client.Send(message);
            }
            catch
            {
                model.WasNotSuccessful();
                return View(model);
            }

            return RedirectToAction(nameof(Contact), new { success = true });
        }

        [HttpGet("/poker")]
        public IActionResult Poker()
        {
            return View();
        }

        private SmtpClient MakeClient()
        {
            var email = Configuration.GetSection("EmailCredentials:Email");
            var password = Configuration.GetSection("EmailCredentials:Password");

            if (email is null || password is null)
            {
                throw new Exception("Não foi possível obter credenciais de envio de email. Entre em contato com o suporte.");
            }

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                EnableSsl = true,
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(email.Value, password.Value)
            };

            return smtp;
        }
    }
}
