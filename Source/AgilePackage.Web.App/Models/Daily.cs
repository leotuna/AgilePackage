using System;

namespace AgilePackage.Web.App.Models
{
    public class Daily : Entity
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }

        public Daily()
        {
        }

        public Daily(Guid projectId, Guid userId, string content)
        {
            ProjectId = projectId;
            UserId = userId;
            Content = content;
        }
    }
}
