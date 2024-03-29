﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Warehouse.Filter
{
    public class RepeatRequest : ActionFilterAttribute
    {
        string key { get; }
        string route { get; }
        public RepeatRequest(string key="Home", string route="Index")
        {
            this.key = key;
            this.route = route;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var _cache = httpContext.RequestServices.GetService<IMemoryCache>();
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _cache.TryGetValue(userId, out string value);
            if (userId == null || value != null)
            {
                context.Result = new RedirectToRouteResult(
                     new RouteValueDictionary {
                    { "controller", key }, { "action", route }
                 });
            }
            var path = httpContext.Request.Path.ToString();
            _cache.Set(userId, path, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(4)
            });
        }
    }
}
