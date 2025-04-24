# DateTimeTagHelper

A custom ASP.NET Core TagHelper for rendering `DateTime` values in Razor views, with support for time zone conversion, customizable formatting, and CSS styling.

## Purpose
This tag helper was created to **centralize** datetime conversion and rendering. In business-critical applications, datetimes can be essential. This tag helper ensures datetimes are converted correctly and displayed accurately. 

## Features

- Convert `DateTime` values to a specified time zone.
- Support for custom date and time formatting.
- Optionally display or hide time zone abbreviations.
- Add custom CSS classes to rendered output.

## Installation

1. Add the `DateTimeTagHelper` to your project.
2. Register the `DateTimeTagHelperOptions` in `Program.cs`:

   ```csharp
   using DateTimeTagHelper.Configuration;

   var builder = WebApplication.CreateBuilder(args);

   // Add services to the container.
   builder.Services.Configure<DateTimeTagHelperOptions>(options =>
   {
       options.DefaultTimeZone = "America/New_York"; // Replace with your desired default time zone.
       options.Default24Hr = true;                  // Use 24-hour format by default.
       options.DefaultHideTimeZone = false;         // Show timezone abbreviations by default.
   });

   var app = builder.Build();
   ```

3. Add the namespace to your `_ViewImports.cshtml` file:

   ```html
   @addTagHelper *, DateTimeTagHelper
   ```

## Usage

### Basic Example

Render a `DateTime` in UTC:

```html
<date-time utc="@new DateTime(2025, 3, 15, 10, 30, 0)"></date-time>
```

Output:
```
2025-03-15 10:30:00 AM UTC
```

### Specify Time Zone

Convert `DateTime` to a specific time zone:

```html
<date-time utc="@new DateTime(2025, 3, 15, 10, 30, 0)" tz="@(TimeZoneInfo.FindSystemTimeZoneById("America/New_York"))"></date-time>
```

Output:
```
2025-03-15 6:30:00 AM ET
```

### Custom Format

Specify a custom format for rendering:

```html
<date-time utc="@new DateTime(2025, 3, 15, 10, 30, 0)" format="MMMM dd, yyyy hh:mm tt"></date-time>
```

Output:
```
March 15, 2025 10:30 AM UTC
```

### 24-Hour Format

Force the 24-hour format:

```html
<date-time 24hr utc="@new DateTime(2025, 3, 15, 10, 30, 0)"></date-time>
```

Output:
```
2025-03-15 10:30:00 UTC
```

### Hide Time Zone

Hide the time zone abbreviation:

```html
<date-time hide-tz utc="@new DateTime(2025, 3, 15, 10, 30, 0)"></date-time>
```

Output:
```
2025-03-15 10:30:00
```

### Add CSS Classes

Add custom CSS classes for the `DateTime` and time zone:

```html
<date-time utc="@new DateTime(2025, 3, 15, 10, 30, 0)" dt-class="text-muted" tz-class="fw-bold"></date-time>
```

## Configuration Options

You can set default options for the `DateTimeTagHelper` in `Program.cs`:

- `DefaultTimeZone`: The fallback time zone if none is specified.
- `Default24Hr`: Whether to default to 24-hour time format.
- `DefaultHideTimeZone`: Whether to hide time zone abbreviations by default.

Example:

```csharp
builder.Services.Configure<DateTimeTagHelperOptions>(options =>
{
    options.DefaultTimeZone = "America/New_York";
    options.Default24Hr = true;
    options.DefaultHideTimeZone = false;
});
```

## License

This project is licensed under the MIT License.
