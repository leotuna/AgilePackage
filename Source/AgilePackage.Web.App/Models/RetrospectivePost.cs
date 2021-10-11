using AgilePackage.Web.App.Enums;
using System;
using System.Collections.Generic;

namespace AgilePackage.Web.App.Models
{
    public class RetrospectivePost : Entity
    {
        public Guid RetrospectiveId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public RetrospectiveType Type { get; set; }
        public List<RetrospectivePostVote> Votes { get; set; }
    }
}
