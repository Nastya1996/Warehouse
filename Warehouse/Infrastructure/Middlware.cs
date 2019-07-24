using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Infrastructure
{
    public class Middlware
    {
        RequestDelegate _next;
        public Middlware(RequestDelegate next) => _next = next;
        public async Task Invoke(HttpContext httpContext)
        {
            
            await _next.Invoke(httpContext);
        }
    }
}
