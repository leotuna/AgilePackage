using AgilePackage.Web.App.Data;
using AgilePackage.Web.App.Dtos;
using AgilePackage.Web.App.Extensions;
using AgilePackage.Web.App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AgilePackage.Web.App.Controllers
{
    [Route("/poker-plannings")]
    [AllowAnonymous]
    public class PokerPlanningController : AgilePackageBaseController
    {
        public IConfiguration Configuration { get; }
        public AgilePackageDbContext DataContext { get; }

        public PokerPlanningController(
            IConfiguration configuration,
            AgilePackageDbContext appDbContext)
        {
            Configuration = configuration;
            DataContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string room)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await DataContext.Users.Select(x => new { x.Id, x.Name }).FirstOrDefaultAsync(x => x.Id == this.GetCurrentUserId());

                if (user is null)
                {
                    return View();
                }

                this.SetVisitantUser(user.Id, user.Name);

                if (string.IsNullOrWhiteSpace(room))
                {
                    return RedirectToAction(nameof(Rooms));
                }

                return RedirectToAction(nameof(Room), new { room });
            }

            try
            {
                var visitantUser = this.GetVisitantUser();
                return View(visitantUser);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(VisitantUser model, [FromQuery] string room)
        {
            var lead = new Lead(model.Name, model.Email);

            await DataContext.Leads.AddAsync(lead);

            await DataContext.SaveChangesAsync();

            this.SetVisitantUser(lead.Id, lead.Name, lead.Email);

            if (string.IsNullOrWhiteSpace(room))
            {
                return RedirectToAction(nameof(Rooms));
            }

            return RedirectToAction(nameof(Room), new { room });
        }

        [HttpGet("rooms")]
        public IActionResult Rooms([FromQuery] string room = "")
        {
            try
            {
                this.GetVisitantUser();
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(room))
            {
                return View();
            }
            else
            {
                return View(new SelectRoomDto { Name = room });
            }
        }

        [HttpPost("rooms")]
        public IActionResult Rooms(SelectRoomDto model)
        {
            var roomNameIsInvalid = !Uri.IsWellFormedUriString(model.Name, UriKind.Relative);

            if (roomNameIsInvalid)
            {
                this.ToastError("This is not a valid url.");
                return RedirectToAction(nameof(Rooms));
            }

            return RedirectToAction(nameof(Room), new { room = model.Name });
        }

        [HttpGet("{room}")]
        public async Task<IActionResult> Room(string room)
        {
            VisitantUser visitantUser;
            try
            {
                visitantUser = this.GetVisitantUser();

                var leadRoom = new LeadRoom(visitantUser.Id, room);

                await DataContext.LeadRooms.AddAsync(leadRoom);

                await DataContext.SaveChangesAsync();
            }
            catch
            {
                return RedirectToAction(nameof(Index), new { room });
            }

            return View(new RoomDto() { Name = room, User = visitantUser.Name });
        }

        [HttpGet("bye")]
        public IActionResult Bye(string room)
        {
            return View(new ByeDto { Room = room });
        }
    }
}
