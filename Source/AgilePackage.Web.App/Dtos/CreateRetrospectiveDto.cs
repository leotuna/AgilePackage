using System;

namespace AgilePackage.Web.App.Dtos
{
    public class CreateRetrospectiveDto
    {
        public string Title { get; set; }
        public Guid ProjectId { get; set; }

        public Models.Retrospective ConvertToDomain(Guid projectId)
        {
            return new Models.Retrospective
            {
                ProjectId = projectId,
                Title = Title,
            };
        }
    }
}
