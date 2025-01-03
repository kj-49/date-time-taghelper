using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneNames;

namespace DateTimeTagHelper.TagHelpers;
public class DateTimeTagHelper : TagHelper
{
    // DateTime in UTC (nullable)
    public DateTime? Utc { get; set; }

    // Timezone to display the DateTime in
    public TimeZoneInfo Tz { get; set; }

    // The format in which to render the DateTime, this will override the 24hr setting
    public string? Format { get; set; }

    [HtmlAttributeName("24hr")]
    public bool _24Hr { get; set; }

    // Whether to hide the timezone abbreviation
    [HtmlAttributeName("hide-tz")]
    public bool HideTimeZone { get; set; } = false;

    // CSS class for the formatted DateTime
    [HtmlAttributeName("dt-class")]
    public string? DateTimeClass { get; set; }

    // CSS class for the TimeZone abbreviation
    [HtmlAttributeName("tz-class")]
    public string? TimeZoneClass { get; set; }

    // Process method to render the DateTime
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // If Utc is null, display "Never" with no timezone
        if (Utc == null)
        {
            output.Content.SetContent("Never");
            return;
        }

        if (Tz is null) Tz = TimeZoneInfo.Utc;

        // Convert DateTime to the specified TimeZone
        var convertedDateTime = TimeZoneInfo.ConvertTime(Utc.Value, Tz);

        string formattedDateTime;

        if (!string.IsNullOrEmpty(Format))
        {
            formattedDateTime = convertedDateTime.ToString(Format);
        }
        else if (_24Hr)
        {
            formattedDateTime = convertedDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        else
        {
            formattedDateTime = convertedDateTime.ToString();
        }

        // Wrap DateTime in a span with a custom class if provided
        if (DateTimeClass is not null)
        {
            formattedDateTime = $"<span class='{DateTimeClass}'>{formattedDateTime}</span>";
        }

        // If HideTimeZone is not set, append the timezone abbreviation
        if (!HideTimeZone)
        {
            var timeZoneAbbreviation = GetTimeZoneAbbreviation(Tz);
            if (TimeZoneClass is not null)
            {
                timeZoneAbbreviation = $"<span class='{TimeZoneClass}'>{timeZoneAbbreviation}</span>";
            }

            formattedDateTime += $" {timeZoneAbbreviation}";
        }

        // Set the tag's content to the formatted DateTime
        output.Content.SetHtmlContent(formattedDateTime);
    }

    private string? GetTimeZoneAbbreviation(TimeZoneInfo timeZone)
    {
        return TZNames.GetAbbreviationsForTimeZone(timeZone.Id, "en-US").Generic;
    }
}

