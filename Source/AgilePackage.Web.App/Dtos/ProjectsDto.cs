using System.Collections.Generic;

namespace AgilePackage.Web.App.Dtos
{
    public class ProjectsDto
    {
        public int Invites { get; set; } = 0;
        public List<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
    }
}
