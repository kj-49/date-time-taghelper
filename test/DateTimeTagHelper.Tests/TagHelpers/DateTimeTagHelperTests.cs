using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using Xunit;
using DateTimeTagHelper.TagHelpers;
using DateTimeTagHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Options;

namespace DateTimeTagHelper.Tests.TagHelpers;

public class DateTimeTagHelperTests
{
    private static TagHelperContext MakeContext() => new TagHelperContext(
        new TagHelperAttributeList(),
        new Dictionary<object, object?>(),
        Guid.NewGuid().ToString()
    );

    private static TagHelperOutput MakeOutput()
    {
        return new TagHelperOutput("date-time",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });
    }

    private static DateTimeTagHelper.TagHelpers.DateTimeTagHelper CreateTagHelper(
        DateTimeTagHelperOptions? options = null,
        IHttpContextAccessor? httpContextAccessor = null)
    {
        var opts = options ?? new DateTimeTagHelperOptions
        {
            DefaultTimeZone = "UTC",
            Default24Hr = false,
            DefaultHideTimeZone = false
        };
        var optionsMock = new Mock<IOptions<DateTimeTagHelperOptions>>();
        optionsMock.Setup(o => o.Value).Returns(opts);

        return new DateTimeTagHelper.TagHelpers.DateTimeTagHelper(optionsMock.Object, httpContextAccessor);
    }

    [Fact]
    public void Process_ShouldDisplayNever_WhenUtcIsNull()
    {
        // Arrange
        var helper = CreateTagHelper();
        var context = MakeContext();
        var output = MakeOutput();

        // Act
        helper.Process(context, output);

        // Assert
        Assert.Equal("Never", output.Content.GetContent());
    }

    [Fact]
    public void Process_ShouldUseDefaultTimeZone_WhenNoneProvided()
    {
        // Arrange
        var utc = new DateTime(2024, 5, 1, 12, 0, 0, DateTimeKind.Utc);
        var helper = CreateTagHelper();
        helper.Utc = utc;
        var context = MakeContext();
        var output = MakeOutput();

        // Act
        helper.Process(context, output);

        // Assert
        var result = output.Content.GetContent();
        Assert.Contains("2024", result); // Date string rendered
        Assert.Contains("UTC", result);  // Abbreviation added
    }

    [Fact]
    public void Process_ShouldUseUserTimeZone_FromHttpContext()
    {
        // Arrange
        var utc = new DateTime(2024, 5, 1, 12, 0, 0, DateTimeKind.Utc);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserTimeZone"] = tz;

        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(a => a.HttpContext).Returns(httpContext);

        var helper = CreateTagHelper(httpContextAccessor: accessor.Object);
        helper.Utc = utc;

        var context = MakeContext();
        var output = MakeOutput();

        // Act
        helper.Process(context, output);

        // Assert
        var result = output.Content.GetContent();
        Assert.Contains("ET", result); // Abbreviation
        Assert.Contains("2024", result);
    }

    [Fact]
    public void Process_ShouldUseCustomFormat_WhenProvided()
    {
        var utc = new DateTime(2024, 5, 1, 12, 0, 0, DateTimeKind.Utc);
        var helper = CreateTagHelper();
        helper.Utc = utc;
        helper.Format = "yyyy/MM/dd HH:mm";

        var context = MakeContext();
        var output = MakeOutput();

        helper.Process(context, output);

        var result = output.Content.GetContent();
        Assert.Contains("2024/05/01 12:00", result);
    }

    [Fact]
    public void Process_ShouldUse24HrFormat_WhenEnabled()
    {
        var utc = new DateTime(2024, 5, 1, 14, 0, 0, DateTimeKind.Utc);
        var helper = CreateTagHelper();
        helper.Utc = utc;
        helper._24Hr = true;

        var context = MakeContext();
        var output = MakeOutput();

        helper.Process(context, output);

        var result = output.Content.GetContent();
        Assert.Contains("14:00:00", result);
    }

    [Fact]
    public void Process_ShouldHideTimeZone_WhenRequested()
    {
        var utc = new DateTime(2024, 5, 1, 12, 0, 0, DateTimeKind.Utc);
        var helper = CreateTagHelper();
        helper.Utc = utc;
        helper.HideTimeZone = true;

        var context = MakeContext();
        var output = MakeOutput();

        helper.Process(context, output);

        var result = output.Content.GetContent();
        Assert.DoesNotContain("UTC", result);
    }

    [Fact]
    public void Process_ShouldUseCustomClasses()
    {
        var utc = new DateTime(2024, 5, 1, 12, 0, 0, DateTimeKind.Utc);
        var helper = CreateTagHelper();
        helper.Utc = utc;
        helper.DateTimeClass = "dt";
        helper.TimeZoneClass = "tz";

        var context = MakeContext();
        var output = MakeOutput();

        helper.Process(context, output);

        var result = output.Content.GetContent();
        Assert.Contains("class='dt'", result);
        Assert.Contains("class='tz'", result);
    }
}
