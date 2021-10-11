using System;

namespace AgilePackage.Web.App.ViewModels
{
    public class SessionRedirectViewModel
    {
        public string SessionId { get; internal set; }

        public SessionRedirectViewModel(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }
            SessionId = sessionId;
        }
    }
}
