using System;
using System.Collections.Generic;
using System.Linq;

namespace AgilePackage.Web.App.Models
{
    public class LiveRetrospective
    {
        public List<UserInLiveRetrospective> Users { get; internal set; } = new List<UserInLiveRetrospective>();

        public void AddUser(UserInLiveRetrospective user)
        {
            Users.Add(user);
        }

        public void RemoveUser(UserInLiveRetrospective user)
        {
            Users.Remove(user);
        }

        public void RemoveUser(string connectionId)
        {
            var user = Users.FirstOrDefault(x => x.ConnectionId == connectionId);
            if (user is null)
            {
                return;
            }
            Users.Remove(user);
        }

        public string GetRetrospectiveIdFromConnectionId(string connectionId)
        {
            var user = Users.FirstOrDefault(x => x.ConnectionId == connectionId);
            if (user is null)
            {
                throw new Exception("User not found.");
            }
            return user.RetrospectiveId.ToString();
        }
    }

    public class UserInLiveRetrospective
    {
        public Guid UserId { get; set; }
        public Guid RetrospectiveId { get; set; }
        public string ConnectionId { get; set; }
    }
}
