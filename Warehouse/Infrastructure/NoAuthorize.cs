using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Infrastructure
{
    public class NoAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var roles = context.HttpContext.User;
            bool auth = roles.IsInRole("Admin") || roles.IsInRole("Worker") || roles.IsInRole("Storekeeper") || roles.IsInRole("Report");
            if (auth)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                { "controller", "Home" }, { "action", "Index" }
                });
            }
        }
    }
}
