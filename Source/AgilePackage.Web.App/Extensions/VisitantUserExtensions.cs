using AgilePackage.Web.App.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AgilePackage.Web.App.Extensions
{
    public static class VisitantUserExtensions
    {
        private static string Id { get; } = "AgilePackage.Id";
        private static string Name { get; } = "AgilePackage.Name";
        private static string Email { get; } = "AgilePackage.Email";

        public static void SetVisitantUser(this Controller controller, Guid id, string name)
        {
            if (id == Guid.Empty)
            {
                throw new Exception();
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception();
            }

            controller.HttpContext.Response.Cookies.Append(Id, id.ToString());
            controller.HttpContext.Response.Cookies.Append(Name, name);
        }

        public static void SetVisitantUser(this Controller controller, Guid id, string name, string email)
        {
            if (id == Guid.Empty)
            {
                throw new Exception();
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception();
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception();
            }

            controller.HttpContext.Response.Cookies.Append(Id, id.ToString());
            controller.HttpContext.Response.Cookies.Append(Name, name);
            controller.HttpContext.Response.Cookies.Append(Email, email);
        }

        public static VisitantUser GetVisitantUser(this Controller controller)
        {
            var idAsString = controller.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == Id)!.Value;
            var name = controller.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == Name)!.Value;
            var email = controller.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == Email)!.Value;

            if (string.IsNullOrWhiteSpace(idAsString))
            {
                throw new Exception();
            }

            var id = Guid.Parse(idAsString);

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception();
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return new VisitantUser(id, name);
            }

            return new VisitantUser(id, name, email);
        }
    }
}
