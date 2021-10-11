using AgilePackage.Web.App.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AgilePackage.Web.App.Controllers
{
    [Authorize]
    public abstract class AgilePackageBaseController : Controller
    {
        internal Guid CurrentUserId { get => this.GetCurrentUserId(); }
    }
}
