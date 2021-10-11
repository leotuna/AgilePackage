using AgilePackage.Core.Services;
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
    [Authorize]
    [Route("/projects/{projectId:guid}/invites")]
    public class ProjectInviteController : AgilePackageBaseController
    {
        private AgilePackageDbContext DataContext { get; }
        private UserBelongsToProjectService ProjectService { get; }
        private EmailService EmailService { get; }
        private IConfiguration Configuration { get; }

        public ProjectInviteController(
            AgilePackageDbContext dataContext,
            UserBelongsToProjectService projectService,
            EmailService emailService,
            IConfiguration configuration)
        {
            DataContext = dataContext;
            ProjectService = projectService;
            EmailService = emailService;
            Configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid projectId, ProjectDetailsDto obj)
        {
            var model = obj.CreateInvite;

            var userBelongsToProject = await ProjectService.ValidateAsync(projectId, CurrentUserId);
            if (!userBelongsToProject.UserIsProjectAdmin)
            {
                this.ToastError("You cannot invite a member to a project if you are not its admin.");
                return RedirectToAction(nameof(ProjectController.Details), typeof(ProjectController).ControllerName(), new { ProjectId = projectId });
            }

            var user = await DataContext
                .Users
                .Select(x => new { x.Id, x.Email })
                .FirstOrDefaultAsync(x => x.Email == model.ToEmail);

            if (user is not null)
            {
                var invitedUserAlreadyBelongsToProject = await DataContext.ProjectUsers.AnyAsync(x => x.UserId == user.Id && x.ProjectId == projectId);
                if (invitedUserAlreadyBelongsToProject)
                {
                    this.ToastError("User already belongs to project.");
                    return RedirectToAction(nameof(ProjectController.Details), typeof(ProjectController).ControllerName(), new { ProjectId = projectId });
                }
            }

            await DataContext.Invites.AddAsync(new Invite
            {
                ProjectId = projectId,
                CreatedByUserId = CurrentUserId,
                ToEmail = model.ToEmail,
                ToUserId = user?.Id,
            });

            await DataContext.SaveChangesAsync();

            var createAccountUrl = string.Concat(Configuration["Urls:App"], "/sign-up?invite=true");

            var signInUrl = string.Concat(Configuration["Urls:App"], "/sign-in?invite=true");

            var callToAction = user is null ? $"Create your account <a href={createAccountUrl}>clicking here</a>" : $"<a href={signInUrl}>Log in to your account</a> to accept the invite";

            EmailService.Send(
                to: model.ToEmail,
                subject: "Project invite",
                body: $"<p>You were invited to a project in Agile Package.</p><p>{callToAction}</p>.",
                isBodyHtml: true);

            this.ToastSuccess("Invite created!");

            return RedirectToAction(nameof(ProjectController.Details), typeof(ProjectController).ControllerName(), new { ProjectId = projectId });
        }

        [HttpGet("{inviteId:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid projectId, Guid inviteId)
        {
            var userBelongsToProject = await ProjectService.ValidateAsync(projectId, CurrentUserId);
            if (!userBelongsToProject.UserIsProjectAdmin)
            {
                this.ToastError("You cannot delete this invite if you are not a project admin.");
                return RedirectToAction(nameof(ProjectController.Details), typeof(ProjectController).ControllerName(), new { ProjectId = projectId });
            }

            var invite = await DataContext.Invites.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.Id == inviteId);
            if (!invite.CanCancel())
            {
                this.ToastError("This invite cannot be deleted anymore.");
                return RedirectToAction(nameof(ProjectController.Details), typeof(ProjectController).ControllerName(), new { ProjectId = projectId });
            }

            invite.Cancel();

            DataContext.Invites.Update(invite);

            await DataContext.SaveChangesAsync();

            this.ToastSuccess("Invite canceled.");

            return RedirectToAction(nameof(ProjectController.Details), typeof(ProjectController).ControllerName(), new { ProjectId = projectId });
        }
    }
}
