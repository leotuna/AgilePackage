using Microsoft.AspNetCore.Identity;
using System;

namespace AgilePackage.Web.App
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; } = string.Empty;

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}
