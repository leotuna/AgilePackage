using System.ComponentModel.DataAnnotations;

namespace AgilePackage.Web.App.Dtos
{
    public class CreateInviteDto
    {
        [EmailAddress]
        [Required]
        public string ToEmail { get; set; }
    }
}
