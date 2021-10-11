using System;
using System.Collections.Generic;

namespace AgilePackage.Web.App.Dtos
{
    public class ProjectDetailsDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<ProjectDetailsMemberDto> Members { get; set; }

        public List<ProjectInvitesDto> Invites { get; set; }

        public CreateInviteDto CreateInvite { get; set; }
    }

    public class ProjectDetailsMemberDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class ProjectInvitesDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public bool CanCancel { get; set; }
    }
}
