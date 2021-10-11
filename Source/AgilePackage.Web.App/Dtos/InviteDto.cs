using System;

namespace AgilePackage.Web.App.Dtos
{
    public class InviteDto
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; }
        public bool Accepted { get; set; }
        public bool Refused { get; set; }
    }
}
