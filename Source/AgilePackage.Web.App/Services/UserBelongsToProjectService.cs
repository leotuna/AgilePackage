using AgilePackage.Web.App.Data;
using AgilePackage.Web.App.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AgilePackage.Core.Services
{
    public class UserBelongsToProjectService
    {
        private AgilePackageDbContext DbContext { get; }

        public UserBelongsToProjectService(AgilePackageDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<UserBelongsToProjectDto> ValidateAsync(Guid projectId, Guid userId)
        {
            var userProject = await DbContext.ProjectUsers.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);

            if (userProject is null)
            {
                return new UserBelongsToProjectDto { UserBelongs = false, UserIsProjectAdmin = false };
            }

            return new UserBelongsToProjectDto { UserBelongs = true, UserIsProjectAdmin = userProject.Admin };
        }
    }
}
