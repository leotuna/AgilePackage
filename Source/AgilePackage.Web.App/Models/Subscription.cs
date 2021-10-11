using System;
using System.ComponentModel.DataAnnotations;

namespace AgilePackage.Web.App.Models
{
    public class Subscription : Entity
    {
        [Required]
        public Guid UserId { get; set; }
        public string CustomerId { get; set; }
    }
}
