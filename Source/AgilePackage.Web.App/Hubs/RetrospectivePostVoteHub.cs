using AgilePackage.Web.App.Data;
using AgilePackage.Web.App.Extensions;
using AgilePackage.Web.App.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AgilePackage.Web.App.Hubs
{
    public class RetrospectivePostVoteHub : Hub
    {
        private Guid CurrentUserId { get => this.GetCurrentUserId(); }
        public string ConnectionId { get => Context.ConnectionId; }
        private LiveRetrospective Retrospective { get; }
        private AgilePackageDbContext RetrospectiveDbContext { get; }

        public RetrospectivePostVoteHub(
            AgilePackageDbContext retrospectiveDbContext,
            LiveRetrospective retrospective)
        {
            RetrospectiveDbContext = retrospectiveDbContext;
            Retrospective = retrospective;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override async Task<Task> OnDisconnectedAsync(Exception? exception)
        {
            var retrospectiveId = Retrospective.GetRetrospectiveIdFromConnectionId(ConnectionId);

            Retrospective.RemoveUser(ConnectionId);

            await Groups.RemoveFromGroupAsync(ConnectionId, retrospectiveId);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task EnterInGroup(Guid retrospectiveId)
        {
            await Groups.AddToGroupAsync(ConnectionId, retrospectiveId.ToString());
            Retrospective.AddUser(new UserInLiveRetrospective { UserId = CurrentUserId, RetrospectiveId = retrospectiveId, ConnectionId = ConnectionId });
        }

        public async Task Vote(Guid retrospectivePostId)
        {
            bool userHasVoted;

            var retrospectiveId = Retrospective.GetRetrospectiveIdFromConnectionId(ConnectionId);

            var userVote = await RetrospectiveDbContext.RetrospectivePostVotes.FirstOrDefaultAsync(x => x.RetrospectivePostId == retrospectivePostId && x.UserId == CurrentUserId);
            if (userVote is null)
            {
                userHasVoted = true;
                userVote = new RetrospectivePostVote { UserId = CurrentUserId, RetrospectivePostId = retrospectivePostId };
                await RetrospectiveDbContext.RetrospectivePostVotes.AddAsync(userVote);
            }
            else
            {
                userHasVoted = false;
                RetrospectiveDbContext.RetrospectivePostVotes.Remove(userVote);
            }

            await RetrospectiveDbContext.SaveChangesAsync();

            var voteCount = await RetrospectiveDbContext.RetrospectivePostVotes.CountAsync(x => x.RetrospectivePostId == retrospectivePostId);

            await Clients.Group(retrospectiveId).SendAsync(nameof(Vote), retrospectivePostId, voteCount, userHasVoted);
        }

        public async Task Delete(Guid retrospectivePostId)
        {
            var retrospectiveId = Retrospective.GetRetrospectiveIdFromConnectionId(ConnectionId);
            await Clients.Group(retrospectiveId).SendAsync(nameof(Delete), retrospectivePostId);
        }

        public async Task Edit(Guid retrospectivePostId, string content)
        {
            var retrospectiveId = Retrospective.GetRetrospectiveIdFromConnectionId(ConnectionId);
            await Clients.Group(retrospectiveId).SendAsync(nameof(Edit), retrospectivePostId, content);
        }
    }
}
