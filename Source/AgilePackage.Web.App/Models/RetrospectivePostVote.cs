using System;

namespace AgilePackage.Web.App.Models
{
    public class RetrospectivePostVote : Entity
    {
        public Guid UserId { get; set; }
        public Guid RetrospectivePostId { get; set; }
    }
}
