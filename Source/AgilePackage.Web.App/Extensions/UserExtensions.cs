using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;

namespace AgilePackage.Web.App.Extensions
{
    public static class UserExtensions
    {
        public static Guid GetCurrentUserId(this ControllerBase controller)
        {
            try
            {
                var userId = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.Parse(userId);
            }
            catch
            {
                throw new Exception("Cannot find user id.");
            }
        }

        public static Guid GetCurrentUserId(this ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.Parse(userId);
            }
            catch
            {
                throw new Exception("Cannot find user id.");
            }
        }

        public static Guid GetCurrentUserId(this Hub hub)
        {
            try
            {
                var userId = hub.Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.Parse(userId);
            }
            catch
            {
                throw new Exception("Cannot find user id.");
            }
        }

        public static bool UserIsAuthenticated(this Controller controller)
        {
            return controller.User.Identity.IsAuthenticated;
        }
    }
}
