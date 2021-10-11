using AgilePackage.Web.App.Enums;
using AgilePackage.Web.App.Models;
using System;

namespace AgilePackage.Web.App.Dtos
{
    public class CreateRetrospectivePostDto
    {
        public Guid RetrospectivePostId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid RetrospectiveId { get; set; }
        public string Content { get; set; }
        public RetrospectiveType Type { get; set; }

        public RetrospectivePost ConvertToDomain(Guid userId, Guid retrospectiveId, RetrospectiveType type)
        {
            return new RetrospectivePost
            {
                UserId = userId,
                RetrospectiveId = retrospectiveId,
                Content = Content,
                Type = type,
            };
        }

        public void UpdateDomain(Models.RetrospectivePost domain)
        {
            domain.Content = Content;
        }
    }
}
