using AgilePackage.Web.App.Data;
using AgilePackage.Web.App.Dtos;
using AgilePackage.Web.App.Enums;
using AgilePackage.Web.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AgilePackage.Web.App.Controllers
{
    [Route("/invites")]
    public class InviteController : AgilePackageBaseController
    {
        private AgilePackageDbContext DataContext { get; }

        public InviteController(
            AgilePackageDbContext dataContext)
        {
            DataContext = dataContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await AddUserIdsToEmptyInvites(CurrentUserId);

            var invites = await DataContext
                .Invites
                .Include(x => x.Project)
                .Where(x => x.ToUserId == CurrentUserId && x.Status != InviteStatus.Canceled)
                .Select(x => new InviteDto
                {
                    Id = x.Id,
                    ProjectName = x.Project.Name,
                    Accepted = x.Status == InviteStatus.Accepted,
                    Refused = x.Status == InviteStatus.Refused,
                })
                .ToListAsync();

            return View(invites);
        }

        [HttpGet("{inviteId:guid}/{method}")]
        public async Task<IActionResult> Edit(Guid inviteId, string method)
        {
            var invite = await DataContext
                .Invites
                .FirstOrDefaultAsync(x => x.Id == inviteId);

            if (invite.ToUserId != CurrentUserId)
            {
                return Forbid();
            }

            if (method == "accept")
            {
                invite.Accept();

                await DataContext.ProjectUsers.AddAsync(new ProjectUser
                {
                    ProjectId = invite.ProjectId,
                    UserId = CurrentUserId,
                });
            }
            else if (method == "refuse")
            {
                invite.Refuse();
            }

            DataContext.Invites.Update(invite);

            await DataContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task AddUserIdsToEmptyInvites(Guid userId)
        {
            var userEmail = await DataContext.Users.Select(x => new { x.Id, x.Email }).FirstOrDefaultAsync(x => x.Id == userId);

            if (userEmail is null)
            {
                throw new Exception();
            }

            var emptyInvites = await DataContext.Invites.Where(x => x.ToUserId == null && x.ToEmail == userEmail.Email).ToListAsync();

            var inviteEmails = emptyInvites.Select(x => x.ToEmail).ToList();

            var users = await DataContext.Users.Select(x => new { x.Id, x.Email }).Where(x => inviteEmails.Contains(x.Email)).ToListAsync();

            var realUsersEmails = users.Select(x => x.Email).Distinct().ToList();

            emptyInvites = emptyInvites.Where(x => realUsersEmails.Contains(x.ToEmail)).ToList();

            foreach (var invite in emptyInvites)
            {
                var user = users.FirstOrDefault(x => x.Email == invite.ToEmail);
                if (user is null)
                {
                    continue;
                }
                invite.ToUserId = user.Id;
            }

            DataContext.Invites.UpdateRange(emptyInvites);

            await DataContext.SaveChangesAsync();
        }
    }
}
