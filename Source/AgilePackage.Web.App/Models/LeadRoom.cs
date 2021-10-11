using System;

namespace AgilePackage.Web.App.Models
{
    public class LeadRoom : Entity
    {
        public Guid LeadId { get; set; }
        public string Room { get; set; }

        public LeadRoom(Guid leadId, string room)
        {
            LeadId = leadId;
            Room = room;
        }
    }
}
