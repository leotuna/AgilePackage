using AgilePackage.Core.Services;
using AgilePackage.Web.App.Data;
using AgilePackage.Web.App.Extensions;
using AgilePackage.Web.App.Models;
using AgilePackage.Web.App.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgilePackage.Web.App.Controllers
{
    [Route("/projects/{projectId:guid}/dailies")]
    public class DailyController : AgilePackageBaseController
    {
        private AgilePackageDbContext DataContext { get; }
        private UserBelongsToProjectService ProjectService { get; }

        public DailyController(AgilePackageDbContext dataContext, UserBelongsToProjectService projectService)
        {
            DataContext = dataContext;
            ProjectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<DailyViewModel>> Index([FromRoute] Guid projectId, [FromQuery] DateTime? date)
        {
            var userBelongsToProject = await ProjectService.ValidateAsync(projectId, CurrentUserId);
            if (!userBelongsToProject.UserBelongs)
            {
                this.ToastError("You do not belong to this project.");
                return RedirectToAction(nameof(ProjectController.Index), typeof(ProjectController).ControllerName());
            }

            if (!date.HasValue)
            {
                date = DateTime.Today;
            }

            var dailies = await DataContext
                .Dailies
                .Where(x => x.ProjectId == projectId && x.CreatedAt.Date == date)
                .OrderByDescending(x => x.CreatedAt)
                .Join(DataContext.Users,
                daily => daily.UserId,
                user => user.Id,
                (daily, user) => new { Daily = daily, User = user })
                .Select(x => new PersonDailyDto
                {
                    Id = x.Daily.Id,
                    Content = x.Daily.Content,
                    UserName = x.User.Name,
                })
                .ToListAsync();

            return View(new DailyViewModel { ProjectId = projectId, Date = date.Value, Dailies = dailies });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Guid projectId, DailyViewModel model)
        {
            var userBelongsToProject = await ProjectService.ValidateAsync(projectId, CurrentUserId);
            if (!userBelongsToProject.UserBelongs)
            {
                this.ToastError("You do not belong to this project.");
                return RedirectToAction(nameof(ProjectController.Index), typeof(ProjectController).ControllerName());
            }

            var daily = new Daily(projectId, CurrentUserId, model.Daily);

            await DataContext.Dailies.AddAsync(daily);

            await DataContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpGet("{dailyId:guid}/delete")]
        public async Task<IActionResult> Delete(Guid projectId, Guid dailyId)
        {
            var daily = await DataContext.Dailies.FindAsync(dailyId);
            if (daily.ProjectId != projectId)
            {
                this.ToastError("This daily does not belongs to this project.");
                return RedirectToAction(nameof(DailyController.Index), typeof(DailyController).ControllerName(), new { projectId });
            }

            var userBelongsToProject = await ProjectService.ValidateAsync(projectId, CurrentUserId);
            if (!userBelongsToProject.UserBelongs)
            {
                this.ToastError("You do not belong to this project.");
                return RedirectToAction(nameof(DailyController.Index), typeof(DailyController).ControllerName(), new { projectId });
            }

            if (daily.UserId != CurrentUserId & userBelongsToProject.UserIsProjectAdmin)
            {
                this.ToastError("You do not own this content.");
                return RedirectToAction(nameof(DailyController.Index), typeof(DailyController).ControllerName(), new { projectId });
            }

            DataContext.Dailies.Remove(daily);

            await DataContext.SaveChangesAsync();

            this.ToastSuccess("Deleted!");

            return RedirectToAction(nameof(DailyController.Index), typeof(DailyController).ControllerName(), new { projectId });
        }
    }
}
