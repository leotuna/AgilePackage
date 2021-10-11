using System;
using System.Collections.Generic;

namespace AgilePackage.Web.App.Models
{
    public class Retrospective : Entity
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public List<RetrospectivePost> Posts { get; set; }
    }
}
