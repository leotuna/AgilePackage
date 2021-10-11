using AgilePackage.Web.App.Data;
using AgilePackage.Web.App.Dtos;
using AgilePackage.Web.App.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgilePackage.Web.App.Controllers
{
    [Route("/projects/{projectId:guid}/retrospectives")]
    public class RetrospectiveController : AgilePackageBaseController
    {
        private AgilePackageDbContext DbContext { get; }

        public RetrospectiveController(AgilePackageDbContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<RetrospectivesDto>> Index(Guid projectId)
        {
            var retrospectives = await DbContext
                .Retrospectives
                .Where(x => x.ProjectId == projectId)
                .Select(x => new RetrospectivesDto
                {
                    Id = x.Id,
                    Title = x.Title,
                })
                .ToListAsync();

            return View(retrospectives);
        }

        [HttpGet("create")]
        public ActionResult<CreateRetrospectiveDto> Create(Guid projectId)
        {
            return View(new CreateRetrospectiveDto { ProjectId = projectId });
        }

        [HttpPost("create")]
        public async Task<ActionResult<CreateRetrospectiveDto>> Create(Guid projectId, CreateRetrospectiveDto model)
        {
            var retrospective = model.ConvertToDomain(projectId);

            await DbContext.Retrospectives.AddAsync(retrospective);

            await DbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpGet("{retrospectiveId:guid}")]
        public async Task<IActionResult> Details(Guid projectId, Guid retrospectiveId)
        {
            var retrospective = await DbContext
                .Retrospectives
                .Include(x => x.Posts)
                .ThenInclude(x => x.Votes)
                .Select(x => new RetrospectiveDto
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    Title = x.Title,
                    WhatWentWell = x.Posts.Select(y => new RetrospectivePostDto
                    {
                        Id = y.Id,
                        UserId = y.UserId,
                        Votes = y.Votes.Count,
                        UserHasVoted = y.Votes.Any(z => z.UserId == CurrentUserId),
                        Content = y.Content,
                        Type = y.Type
                    }).Where(y => y.Type == RetrospectiveType.WhatWentWell).ToList(),
                    WhatWentWrong = x.Posts.Select(y => new RetrospectivePostDto
                    {
                        Id = y.Id,
                        UserId = y.UserId,
                        Votes = y.Votes.Count,
                        UserHasVoted = y.Votes.Any(z => z.UserId == CurrentUserId),
                        Content = y.Content,
                        Type = y.Type
                    }).Where(y => y.Type == RetrospectiveType.WhatWentWrong).ToList(),
                })
                .FirstOrDefaultAsync(x => x.Id == retrospectiveId && x.ProjectId == projectId);

            var userIds = new List<Guid>();
            userIds.AddRange(retrospective.WhatWentWell.Select(x => x.UserId).Distinct().ToList());
            userIds.AddRange(retrospective.WhatWentWrong.Select(x => x.UserId).Distinct().ToList());
            userIds = userIds.Distinct().ToList();

            var userNames = await DbContext.Users.Select(x => new { x.Id, x.Name }).Where(x => userIds.Contains(x.Id)).ToListAsync();

            foreach (var post in retrospective.WhatWentWell)
            {
                post.UserName = userNames.FirstOrDefault(x => x.Id == post.UserId) is null ? string.Empty : userNames.FirstOrDefault(x => x.Id == post.UserId).Name;
            }

            foreach (var post in retrospective.WhatWentWrong)
            {
                post.UserName = userNames.FirstOrDefault(x => x.Id == post.UserId) is null ? string.Empty : userNames.FirstOrDefault(x => x.Id == post.UserId).Name;
            }

            return View(retrospective);
        }
    }
}
