using AgilePackage.Web.App.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgilePackage.Web.App.Hubs
{
    public class RoomHub : Hub
    {
        public string ConnectionId { get => Context.ConnectionId; }
        public List<RoomHubMember> Members { get; set; }

        public RoomHub(
            List<RoomHubMember> members)
        {
            Members = members;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override async Task<Task> OnDisconnectedAsync(Exception? exception)
        {
            var groupName = GetGroupByConnectionId(ConnectionId);

            var memberToRemove = Members.FirstOrDefault(x => x.ConnectionId == ConnectionId);

            Members.Remove(memberToRemove);

            await Reset(groupName);

            await Groups.RemoveFromGroupAsync(ConnectionId, groupName);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task Vote(int value)
        {
            var member = GetMemberByConnectionId(ConnectionId);

            member.SetVote(value);

            var membersFromGroup = GetMembersByGroupName(member.GroupName);

            await Clients.Group(member.GroupName).SendAsync(nameof(Vote), membersFromGroup);
        }

        public async Task Reset(string groupName = "")
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                groupName = GetGroupByConnectionId(ConnectionId);
            }

            var membersFromGroup = GetMembersByGroupName(groupName);

            foreach (var member in membersFromGroup)
            {
                member.Reset();
            }

            await Clients.Group(groupName).SendAsync(nameof(Reset), membersFromGroup);
        }

        public async Task AddToGroup(string groupName, string name)
        {
            await Groups.AddToGroupAsync(ConnectionId, groupName);

            Members.Add(new RoomHubMember() { GroupName = groupName, ConnectionId = ConnectionId, Name = name });

            var members = GetMembersByGroupName(groupName);

            await Clients.Group(groupName).SendAsync(nameof(AddToGroup), members);
        }

        private RoomHubMember GetMemberByConnectionId(string connectionId)
        {
            return Members.FirstOrDefault(x => x.ConnectionId == connectionId);
        }

        private List<RoomHubMember> GetMembersByGroupName(string groupName)
        {
            return Members.Where(x => x.GroupName == groupName).ToList();
        }

        private string GetGroupByConnectionId(string connectionId)
        {
            return Members.FirstOrDefault(x => x.ConnectionId == connectionId)?.GroupName;
        }
    }
}
