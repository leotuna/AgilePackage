using AgilePackage.Core.Services;
using AgilePackage.Web.App.Data;
using AgilePackage.Web.App.Dtos;
using AgilePackage.Web.App.Enums;
using AgilePackage.Web.App.Extensions;
using AgilePackage.Web.App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgilePackage.Web.App.Controllers
{
    [Authorize]
    [Route("/projects")]
    public class ProjectController : AgilePackageBaseController
    {
        private AgilePackageDbContext DataContext { get; }

        public ProjectController(
            AgilePackageDbContext appDbContext)
        {
            DataContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var projects = await DataContext
                .ProjectUsers
                .Include(x => x.Project)
                .Where(x => x.UserId == CurrentUserId)
                .Select(x => new ProjectDto
                {
                    Id = x.ProjectId,
                    Name = x.Project.Name,
                    Slug = x.Project.MakeFriendlySlug(),
                })
                .ToListAsync();

            var inviteCount = await DataContext
                .Invites
                .Where(x => x.ToUserId == CurrentUserId && x.Status == InviteStatus.Pending)
                .CountAsync();

            return View(new ProjectsDto { Projects = projects, Invites = inviteCount });
        }

        [HttpGet("{projectId:guid}")]
        public async Task<IActionResult> Details(Guid projectId, [FromServices] UserBelongsToProjectService projectService)
        {
#warning to do: melhorar query
            var userBelongsToProject = await projectService.ValidateAsync(projectId, CurrentUserId);
            if (!userBelongsToProject.UserBelongs)
            {
                return Forbid("You do not have access to this project.");
            }

            var project = await DataContext
                .Projects
                .Include(x => x.Invites)
                .Select(x => new ProjectDetailsDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Invites = x.Invites.Select(x => new ProjectInvitesDto
                    {
                        Id = x.Id,
                        Email = x.ToEmail,
                        CanCancel = x.CanCancel(),
                    }).ToList(),
                })
                .FirstOrDefaultAsync(x => x.Id == projectId);

            if (project is null)
            {
                return NotFound();
            }

            project.Members = await DataContext
                .ProjectUsers
                .Where(x => x.ProjectId == projectId)
                .Join(DataContext.Users,
                projectUser => projectUser.UserId,
                user => user.Id,
                (projectUser, user) => new { ProjectUser = projectUser, User = user })
                .Select(x => new ProjectDetailsMemberDto
                {
                    Id = x.User.Id,
                    Email = x.User.Email,
                    Name = x.User.Name,
                    IsAdmin = x.ProjectUser.Admin,
                })
                .ToListAsync();

            return View(project);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateProjectDto model)
        {
            var project = Project.Create(model.Name, CurrentUserId);

            await DataContext.Projects.AddAsync(project);

            await DataContext.SaveChangesAsync();

            this.ToastSuccess("Project created!");

            return RedirectToAction(nameof(Index));
        }
    }
}
