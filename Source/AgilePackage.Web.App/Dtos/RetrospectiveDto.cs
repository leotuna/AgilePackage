using AgilePackage.Web.App.Enums;
using System;
using System.Collections.Generic;

namespace AgilePackage.Web.App.Dtos
{
    public class RetrospectiveDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public List<RetrospectivePostDto> WhatWentWell { get; set; } = new List<RetrospectivePostDto>();
        public List<RetrospectivePostDto> WhatWentWrong { get; set; } = new List<RetrospectivePostDto>();
    }

    public class RetrospectivePostDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Votes { get; set; } = 0;
        public bool UserHasVoted { get; set; } = false;
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public RetrospectiveType Type { get; set; }
    }
}
