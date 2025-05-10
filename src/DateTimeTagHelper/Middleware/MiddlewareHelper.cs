using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateTimeTagHelper.Middleware;
public static class MiddlewareHelper
{
    public static IApplicationBuilder UseDateTimeTagHelperMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TimeZoneResolutionMiddleware>();
    }
}
