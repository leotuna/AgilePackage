using System;
using System.Collections.Generic;

namespace AgilePackage.Web.App.Models
{
    public class RoomHubMember
    {
        private List<int> _fibonacci { get; } = new List<int> { 1, 2, 3, 5, 8, 13, 20 };

        public string ConnectionId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public int? Vote { get; private set; }

        public void Reset()
        {
            Vote = null;
        }

        public void SetVote(int vote)
        {
            var isVoteValid = IsVoteValid(vote);

            if (!isVoteValid)
            {
                throw new Exception("Vote is not valid");
            }

            Vote = vote;
        }

        private bool IsVoteValid(int vote)
        {
            var voteIsInFibonacciSequence = _fibonacci.Contains(vote);

            return voteIsInFibonacciSequence;
        }
    }
}
