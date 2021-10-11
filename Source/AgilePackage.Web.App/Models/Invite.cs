using AgilePackage.Web.App.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgilePackage.Web.App.Models
{
    public class Invite : Entity
    {
        [Required]
        public Guid ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        [Required]
        public Guid CreatedByUserId { get; set; }

        [Required]
        public string ToEmail { get; set; }

        public Guid? ToUserId { get; set; }

        public InviteStatus Status { get; internal set; } = InviteStatus.Pending;

        public void Cancel()
        {
            if (!CanCancel())
            {
                throw new Exception("This invite cannot be canceled.");
            }
            Status = InviteStatus.Canceled;
        }

        public void Accept()
        {
            if (Status != InviteStatus.Pending)
            {
                throw new Exception("You can only accept an open invite.");
            }
            Status = InviteStatus.Accepted;
        }

        public void Refuse()
        {
            if (Status != InviteStatus.Pending)
            {
                throw new Exception("You can only refuse an open invite.");
            }
            Status = InviteStatus.Refused;
        }

        public bool CanCancel()
        {
            if (Status == InviteStatus.Refused)
            {
                return false;
            }
            if (Status == InviteStatus.Canceled)
            {
                return false;
            }
            if (Status == InviteStatus.Accepted)
            {
                return false;
            }
            return true;
        }
    }
}
