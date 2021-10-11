using AgilePackage.Core.Services;
using AgilePackage.Web.App.Data;
using AgilePackage.Web.App.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AgilePackage.Web.App.Controllers
{
    [Route("/projects/{projectId:guid}/users")]
    public class ProjectUserController : AgilePackageBaseController
    {
        private AgilePackageDbContext DataContext { get; }
        private UserBelongsToProjectService ProjectService { get; }

        public ProjectUserController(AgilePackageDbContext dataContext, UserBelongsToProjectService projectService)
        {
            DataContext = dataContext;
            ProjectService = projectService;
        }

        [HttpGet("{userId:guid}/delete")]
        public async Task<IActionResult> Delete(Guid projectId, Guid userId)
        {
            var userBelongsToProject = await ProjectService.ValidateAsync(projectId, CurrentUserId);
            if (!userBelongsToProject.UserIsProjectAdmin)
            {
                return Forbid("You cannot delete a member of a project if you are not its admin.");
            }

            var adminUsers = await DataContext.ProjectUsers.CountAsync(x => x.ProjectId == projectId && x.Admin == true);
            if (adminUsers == 1 && CurrentUserId == userId)
            {
                this.ToastError("You are the last admin user. Make someone admin before you leave.");
                return RedirectToAction(nameof(ProjectController.Details), typeof(ProjectController).ControllerName(), new { projectId = projectId });
            }

            var userProject = await DataContext.ProjectUsers.FirstOrDefaultAsync(x => x.UserId == userId && x.ProjectId == projectId);
            if (userProject is null)
            {
                return NotFound();
            }

            DataContext.ProjectUsers.Remove(userProject);

            await DataContext.SaveChangesAsync();

            if (CurrentUserId == userId)
            {
                return RedirectToAction(nameof(ProjectController.Index), typeof(ProjectController).ControllerName());
            }

            return RedirectToAction(nameof(ProjectController.Details), typeof(ProjectController).ControllerName(), new { projectId = projectId });
        }

        [HttpGet("{userId:guid}/admin")]
        public async Task<IActionResult> UpdateUserAdmin(Guid projectId, Guid userId)
        {
            var userBelongsToProject = await ProjectService.ValidateAsync(projectId, CurrentUserId);
            if (!userBelongsToProject.UserIsProjectAdmin)
            {
                return Forbid("You cannot update a member of a project if you are not its admin.");
            }

            var userProject = await DataContext.ProjectUsers.FirstOrDefaultAsync(x => x.UserId == userId && x.ProjectId == projectId);
            if (userProject is null)
            {
                return NotFound();
            }

            var adminUsers = await DataContext.ProjectUsers.CountAsync(x => x.ProjectId == projectId && x.Admin == true);
            if (userProject.Admin == true && adminUsers == 1 && CurrentUserId == userId)
            {
                this.ToastError("You are the last admin user. Make someone admin before you leave.");
                return RedirectToAction(nameof(ProjectController.Details), typeof(ProjectController).ControllerName(), new { projectId = projectId });
            }

            userProject.SwitchAdminStatus();

            DataContext.ProjectUsers.Update(userProject);

            await DataContext.SaveChangesAsync();

            return RedirectToAction(nameof(ProjectController.Details), typeof(ProjectController).ControllerName(), new { projectId = projectId });
        }
    }
}
