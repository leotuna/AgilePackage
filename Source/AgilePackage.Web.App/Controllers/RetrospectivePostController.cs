using AgilePackage.Web.App.Data;
using AgilePackage.Web.App.Dtos;
using AgilePackage.Web.App.Enums;
using AgilePackage.Web.App.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AgilePackage.Web.App.Controllers
{
    [Route("/projects/{projectId:guid}/retrospectives/{retrospectiveId:guid}/posts")]
    public class RetrospectivePostController : AgilePackageBaseController
    {
        private AgilePackageDbContext RetrospectiveDbContext { get; }
        public RetrospectivePostController(
            AgilePackageDbContext retrospectiveDbContext)
        {
            RetrospectiveDbContext = retrospectiveDbContext;
        }

        [HttpGet("create")]
        public ActionResult<CreateRetrospectivePostDto> Create(Guid projectId, Guid retrospectiveId, [FromQuery] bool well)
        {
            return View(new CreateRetrospectivePostDto
            {
                ProjectId = projectId,
                RetrospectiveId = retrospectiveId,
                Type = well == true ? RetrospectiveType.WhatWentWell : RetrospectiveType.WhatWentWrong
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<CreateRetrospectiveDto>> Create(
            [FromRoute] Guid projectId,
            [FromRoute] Guid retrospectiveId,
            [FromForm] CreateRetrospectivePostDto model,
            [FromQuery] bool well)
        {
            var post = model.ConvertToDomain(
                CurrentUserId,
                retrospectiveId,
                type: well == true ? RetrospectiveType.WhatWentWell : RetrospectiveType.WhatWentWrong);

            await RetrospectiveDbContext.RetrospectivePosts.AddAsync(post);

            await RetrospectiveDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(RetrospectiveController.Details), typeof(RetrospectiveController).ControllerName(), new { projectId, retrospectiveId });
        }

        [HttpGet("{retrospectivePostId:guid}/edit")]
        public async Task<ActionResult<CreateRetrospectivePostDto>> Edit(Guid projectId, Guid retrospectiveId, Guid retrospectivePostId)
        {
            var post = await RetrospectiveDbContext
                .RetrospectivePosts
                .FirstOrDefaultAsync(x => x.Id == retrospectivePostId && x.RetrospectiveId == retrospectiveId);

            return View(new CreateRetrospectivePostDto
            {
                RetrospectivePostId = retrospectivePostId,
                ProjectId = projectId,
                RetrospectiveId = retrospectiveId,
                Type = post.Type,
                Content = post.Content,
            });
        }

        [HttpPost("{retrospectivePostId:guid}/edit")]
        public async Task<ActionResult<CreateRetrospectiveDto>> Edit(
            [FromRoute] Guid projectId,
            [FromRoute] Guid retrospectiveId,
            [FromRoute] Guid retrospectivePostId,
            [FromForm] CreateRetrospectivePostDto model,
            [FromQuery] bool well)
        {
            var post = await RetrospectiveDbContext
                .RetrospectivePosts
                .FirstOrDefaultAsync(x => x.Id == retrospectivePostId && x.RetrospectiveId == retrospectiveId);

            model.UpdateDomain(post);

            RetrospectiveDbContext.RetrospectivePosts.Update(post);

            await RetrospectiveDbContext.SaveChangesAsync();

            this.ToastSuccess("Post was edited!");

            return RedirectToAction(nameof(RetrospectiveController.Details), typeof(RetrospectiveController).ControllerName(), new { projectId, retrospectiveId });
        }


        [HttpGet("{retrospectivePostId:guid}/delete")]
        public async Task<IActionResult> Delete(Guid projectId, Guid retrospectiveId, Guid retrospectivePostId)
        {
            var post = await RetrospectiveDbContext
                .RetrospectivePosts
                .FirstOrDefaultAsync(x => x.Id == retrospectivePostId && x.RetrospectiveId == retrospectiveId);

            if (post is null)
            {
                return NotFound();
            }

            RetrospectiveDbContext.RetrospectivePosts.Remove(post);

            await RetrospectiveDbContext.SaveChangesAsync();

            this.ToastSuccess("Post deleted!");

            return RedirectToAction(nameof(RetrospectiveController.Details), typeof(RetrospectiveController).ControllerName(), new { projectId, retrospectiveId });
        }
    }
}
