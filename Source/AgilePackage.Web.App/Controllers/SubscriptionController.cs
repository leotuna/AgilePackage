//using AgilePackage.Web.App.Data;
//using AgilePackage.Web.App.Extensions;
//using AgilePackage.Web.App.ViewModels;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Stripe;
//using Stripe.Checkout;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AgilePackage.Web.App.Controllers
//{
//    [Route("/subscriptions")]
//    public class SubscriptionController : AgilePackageBaseController
//    {
//        private AgilePackageDbContext DataContext { get; }
//        private IConfiguration Configuration { get; }
//        private string BaseUrl { get; set; }
//        private string StripePrivateKey { get; set; }

//        public SubscriptionController(
//            IConfiguration configuration,
//            AgilePackageDbContext dbContext)
//        {
//            Configuration = configuration;
//            DataContext = dbContext;
//            BaseUrl = Configuration["Urls:App"];
//            StripePrivateKey = Configuration["Stripe:PrivateKey"];
//        }

//        [HttpGet]
//        public async Task<IActionResult> Index()
//        {
//            var userHasSubscription = await DataContext.Subscriptions.AnyAsync(x => x.UserId == CurrentUserId);
//            if (userHasSubscription)
//            {
//                return RedirectToAction(nameof(ProjectController.Index), typeof(ProjectController).ControllerName());
//            }
//            return RedirectToAction(nameof(Create));
//        }

//        [HttpGet("create")]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        [HttpGet("session")]
//        public async Task<IActionResult> Session()
//        {
//            var priceId = Configuration["Stripe:PriceId"];

//            if (string.IsNullOrWhiteSpace(StripePrivateKey))
//            {
//                throw new Exception();
//            }
//            if (string.IsNullOrWhiteSpace(priceId))
//            {
//                throw new Exception();
//            }
//            if (string.IsNullOrWhiteSpace(BaseUrl))
//            {
//                throw new Exception();
//            }

//            var user = await DataContext.Users.Select(x => new { x.Email, x.Id }).FirstOrDefaultAsync(x => x.Id == CurrentUserId);

//            if (user is null)
//            {
//                throw new Exception();
//            }

//            StripeConfiguration.ApiKey = StripePrivateKey;

//            var options = new SessionCreateOptions
//            {
//                CustomerEmail = user.Email,
//                SuccessUrl = string.Concat(BaseUrl, "/subscriptions/success?session={CHECKOUT_SESSION_ID}"),
//                CancelUrl = string.Concat(BaseUrl, "/subscriptions/error"),
//                PaymentMethodTypes = new List<string> { "card" },
//                Mode = "subscription",
//                LineItems = new List<SessionLineItemOptions>
//                {
//                    new SessionLineItemOptions { Price = priceId, Quantity = 1, },
//                },
//                SubscriptionData = new SessionSubscriptionDataOptions
//                {
//                    TrialPeriodDays = 30,
//                },
//            };

//            try
//            {
//                var service = new SessionService();
//                var session = await service.CreateAsync(options);
//                return RedirectToAction(nameof(SessionRedirect), new { id = session.Id });
//            }
//            catch (StripeException e)
//            {
//                return BadRequest(e.StripeError.Message);
//            }
//        }

//        [HttpGet("session/redirect")]
//        public ActionResult<SessionRedirectViewModel> SessionRedirect([FromQuery] string id)
//        {
//            return View(new SessionRedirectViewModel(id));
//        }

//        [HttpGet("success")]
//        public async Task<IActionResult> Success([FromQuery] string session)
//        {
//            try
//            {
//                var stripePrivateKey = Configuration["Stripe:PrivateKey"];

//                StripeConfiguration.ApiKey = stripePrivateKey;

//                var service = new SessionService();

//                var stripeSession = await service.GetAsync(session);

//                await DataContext.Subscriptions.AddAsync(new Models.Subscription
//                {
//                    UserId = CurrentUserId,
//                    CustomerId = stripeSession.CustomerId,
//                });

//                await DataContext.SaveChangesAsync();
//            }
//            catch
//            {
//                return RedirectToAction(nameof(Error));
//            }

//            return View();
//        }

//        [HttpGet("error")]
//        public IActionResult Error()
//        {
//            return View();
//        }

//        [HttpGet("portal")]
//        public async Task<IActionResult> Portal()
//        {
//            StripeConfiguration.ApiKey = StripePrivateKey;

//            var subscription = await DataContext.Subscriptions.FirstOrDefaultAsync(x => x.UserId == CurrentUserId);
//            if (subscription is null)
//            {
//                this.ToastError("You don't have a subscription. Create one.");
//                return RedirectToAction(nameof(Create));
//            }

//            var options = new Stripe.BillingPortal.SessionCreateOptions
//            {
//                Customer = subscription.CustomerId,
//                ReturnUrl = BaseUrl,
//            };

//            var service = new Stripe.BillingPortal.SessionService(new StripeClient(StripePrivateKey));

//            var session = await service.CreateAsync(options);

//            return Redirect(session.Url);
//        }
//    }
//}
