using System;

namespace AgilePackage.Web.App.Models
{
    public class VisitantUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public VisitantUser()
        {

        }

        public VisitantUser(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public VisitantUser(Guid id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }
}
