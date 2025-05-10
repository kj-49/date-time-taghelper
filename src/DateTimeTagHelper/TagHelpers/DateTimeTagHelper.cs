using DateTimeTagHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System.Globalization;
using TimeZoneNames;

namespace DateTimeTagHelper.TagHelpers;

/// <summary>
/// A TagHelper for rendering DateTime values, with support for time zones, formatting, and customizing the display.
/// </summary>
public class DateTimeTagHelper : TagHelper
{
    private readonly DateTimeTagHelperOptions _options;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeTagHelper"/> class.
    /// </summary>
    /// <param name="options">The configuration options for the tag helper.</param>
    public DateTimeTagHelper(IOptions<DateTimeTagHelperOptions> options, IHttpContextAccessor? httpContextAccessor)
    {
        _options = options.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets or sets the DateTime in UTC. If null, will display "Never".
    /// </summary>
    public DateTime? Utc { get; set; }

    /// <summary>
    /// Gets or sets the TimeZone to display the DateTime in. If null, the default time zone from options will be used.
    /// </summary>
    public TimeZoneInfo Tz { get; set; }

    /// <summary>
    /// Gets or sets the format in which to render the DateTime. This overrides the 24-hour format setting.
    /// If not provided, a default format is used.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display the time in 24-hour format.
    /// This overrides any other format settings.
    /// </summary>
    [HtmlAttributeName("24hr")]
    public bool _24Hr { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to hide the timezone abbreviation.
    /// Defaults to false.
    /// </summary>
    [HtmlAttributeName("hide-tz")]
    public bool HideTimeZone { get; set; } = false;

    /// <summary>
    /// Gets or sets the CSS class for the formatted DateTime.
    /// </summary>
    [HtmlAttributeName("dt-class")]
    public string? DateTimeClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the TimeZone abbreviation.
    /// </summary>
    [HtmlAttributeName("tz-class")]
    public string? TimeZoneClass { get; set; }

    /// <summary>
    /// Processes the DateTime tag and renders it as an HTML string.
    /// </summary>
    /// <param name="context">The context of the tag helper.</param>
    /// <param name="output">The output of the tag helper, where the result will be written.</param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // If Utc is null, display "Never" with no timezone
        if (Utc is null)
        {
            output.Content.SetContent("Never");
            return;
        }

        // Use the default timezone if none is provided
        if (Tz is null)
        {
            // Try to get the resolved time zone from the current HttpContext
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext?.Items.TryGetValue("UserTimeZone", out var userTz) == true && userTz is TimeZoneInfo tz)
            {
                Tz = tz;
            }
            else
            {
                // Fall back to default
                Tz = TimeZoneInfo.FindSystemTimeZoneById(_options.DefaultTimeZone);
            }
        }

        // Convert DateTime to the specified TimeZone
        var convertedDateTime = TimeZoneInfo.ConvertTimeFromUtc(Utc.Value, Tz);

        string formattedDateTime;

        // Use the provided format or fallback to 24-hour format if specified
        if (!string.IsNullOrEmpty(Format))
        {
            formattedDateTime = convertedDateTime.ToString(Format, CultureInfo.InvariantCulture);
        }
        else if (_24Hr || _options.Default24Hr)
        {
            formattedDateTime = convertedDateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
        else
        {
            formattedDateTime = convertedDateTime.ToString(CultureInfo.InvariantCulture);
        }

        // Wrap DateTime in a span with a custom class if provided
        if (DateTimeClass is not null)
        {
            formattedDateTime = $"<span class='{DateTimeClass}'>{formattedDateTime}</span>";
        }

        // If HideTimeZone is not set, append the timezone abbreviation
        if (!HideTimeZone && !_options.DefaultHideTimeZone)
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

    /// <summary>
    /// Gets the abbreviation for the given time zone.
    /// </summary>
    /// <param name="timeZone">The time zone information.</param>
    /// <returns>The time zone abbreviation, or null if it cannot be found.</returns>
    private string? GetTimeZoneAbbreviation(TimeZoneInfo timeZone)
    {
        return TZNames.GetAbbreviationsForTimeZone(timeZone.Id, "en-US").Generic;
    }
}
