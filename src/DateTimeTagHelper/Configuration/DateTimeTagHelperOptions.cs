using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateTimeTagHelper.Configuration;
public class DateTimeTagHelperOptions
{
    public bool Default24Hr { get; set; } = false;  // Default 24-hour format setting
    public string DefaultTimeZone { get; set; } = "UTC"; // Default time zone
    public bool DefaultHideTimeZone { get; set; } = false; // Default Hide TimeZone setting

    /// <summary>
    /// Optional delegate to resolve the current user's time zone.
    /// This should be fast — ideally it reads from HttpContext.Items or a cache.
    /// </summary>
    public Func<TimeZoneInfo?>? CurrentTimeZoneProvider { get; set; }
}

