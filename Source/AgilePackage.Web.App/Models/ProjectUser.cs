using System;

namespace AgilePackage.Web.App.Models
{
    public class ProjectUser : Entity
    {
        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

        public Guid UserId { get; set; }

        public bool Admin { get; internal set; } = false;

        public ProjectUser()
        {
        }

        public ProjectUser(bool admin = false)
        {
            Admin = admin;
        }

        public void MakeAdmin()
        {
            Admin = true;
        }

        public void RemoveAdmin()
        {
            Admin = false;
        }

        public void SwitchAdminStatus()
        {
            Admin = !Admin;
        }
    }
}
