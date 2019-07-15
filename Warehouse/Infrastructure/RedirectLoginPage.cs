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
    public class RedirectLoginPage : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var roles = context.HttpContext.User;
            if (!(roles.IsInRole("Admin") || roles.IsInRole("Worker") || roles.IsInRole("Storekeeper") || roles.IsInRole("Report")))
            {
                context.Result = new RedirectToRouteResult(
                     new RouteValueDictionary {
                { "area", "Identity" }, { "page", "/Account/Login" }
                 });
            }
        }
    }
}
