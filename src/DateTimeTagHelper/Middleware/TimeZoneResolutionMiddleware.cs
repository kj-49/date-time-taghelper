using DateTimeTagHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DateTimeTagHelper.Middleware;
public class TimeZoneResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly DateTimeTagHelperOptions _options;

    public TimeZoneResolutionMiddleware(RequestDelegate next, IOptions<DateTimeTagHelperOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_options.CurrentTimeZoneProvider is not null)
        {
            var timeZone = _options.CurrentTimeZoneProvider();

            if (timeZone is not null)
            {
                context.Items["UserTimeZone"] = timeZone;
            }
        }

        await _next(context);
    }
}

