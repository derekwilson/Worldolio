# Technology assessment


## SQLite and Dapper

https://medium.com/@erdalkama/exploring-sqlite-integration-in-net-maui-59371a8ec1d3

Using Dapper in .NET MAUI is a common choice for developers seeking high performance and direct control over SQL queries compared to heavier frameworks like Entity Framework Core. 
Core Concepts and Setup

    Purpose: Dapper is a micro-ORM that extends the IDbConnection interface to map database results directly to C# objects.
    Local Storage (SQLite): For mobile and desktop apps, Dapper is typically paired with Microsoft.Data.Sqlite to manage local data persistence.
    Installation: Add the following NuGet packages to your project:
        Dapper
        Microsoft.Data.Sqlite 

Implementation Steps

    Initialize the Database: Since SQLite is file-based, you must define a connection string pointing to the app's local storage path.
    Define Models: Create plain C# classes (POCOs) that mirror your database table structure.
    Perform CRUD Operations: Use Dapper's extension methods like QueryAsync<T> for reading and ExecuteAsync for writing.
    csharp

    // Example: Reading from a local SQLite database
    using (IDbConnection db = new SqliteConnection(connectionString))
    {
        var items = await db.QueryAsync<MyModel>("SELECT * FROM MyTable");
        return items.ToList();
    }

    Use code with caution.
     

Best Practices and Considerations

    Architecture: Avoid connecting directly to a remote SQL Server from a mobile device due to security and connection stability risks. Instead, use an ASP.NET Core Web API as a middle tier.
    Type Handlers: Dapper has limitations with certain types like DateTimeOffset or Guid when used with SQLite; these may require custom Type Handlers.
    Dependency Injection: Register your database services as singletons in MauiProgram.cs to ensure efficient connection management across the app.
    Android Emulator Connection: If connecting to a local server during development, use the IP 10.0.2.2 instead of localhost. 




## NodaTime

https://github.com/nodatime/nodatime


Using NodaTime in .NET MAUI allows for robust date/time handling across different time zones, which is often difficult with the standard DateTime class. Since NodaTime works well with .NET Standard 2.0+ (and .NET 6/7/8+), it integrates seamlessly into MAUI. 
1. Installation
Add the NodaTime package to your MAUI project via NuGet Package Manager: 
bash

dotnet add package NodaTime

Use code with caution.
2. Basic Usage in MAUI (ViewModels/Services)
Use NodaTime types in your ViewModel to represent time accurately. 
csharp

using NodaTime;

// 1. Get current time (Instant is UTC)
Instant now = SystemClock.Instance.GetCurrentInstant();

// 2. Convert to a timezone (e.g., Tokyo)
var tzdb = DateTimeZoneProviders.Tzdb;
DateTimeZone tokyoZone = tzdb["Asia/Tokyo"];
ZonedDateTime tokyoTime = now.InZone(tokyoZone);

// 3. Display as a string in MAUI UI
string display = tokyoTime.ToString("F", System.Globalization.CultureInfo.CurrentCulture);
// Result: Friday, April 26, 2024 10:00:00 AM

Use code with caution.
3. Key NodaTime Types to Use

    Instant: A fixed point in time (use this for storing data, timestamps).
    LocalDate: A date without a time (birthdays, anniversaries).
    LocalDateTime: A date and time without a time zone.
    ZonedDateTime: A specific time in a specific zone (best for scheduling). 

4. Integration with MAUI UI
When displaying NodaTime in a Label, convert it to a string. When accepting user input (e.g., a DatePicker), convert it back to NodaTime. 
Convert DatePicker to LocalDate:
csharp

// In your ViewModel
public void OnDateSelected(DateChangedEventArgs e)
{
    LocalDate selected = LocalDate.FromDateTime(e.NewDate);
}

Use code with caution.
5. Dependency Injection for Testing 
Use IClock for testability, allowing you to mock time, which is essential for app development. 
csharp

// Register in MauiProgram.cs
builder.Services.AddSingleton<IClock>(SystemClock.Instance);

// Inject into ViewModel
public class MyViewModel {
    private readonly IClock _clock;
    public MyViewModel(IClock clock) { _clock = clock; }
}

Use code with caution.
6. Tips for MAUI & NodaTime

    Time Zone Persistence: Store times as UTC in the database, but convert to ZonedDateTime when showing them to the user, using the IANA Time Zone ID.
    Android/iOS Specifics: For grabbing the device's local time zone automatically, you can use DateTimeZoneProviders.Tzdb.GetSystemDefault().
    JSON Serialization: If you are serializing NodaTime objects for an API, add the NodaTime.Serialization.JsonNet package. 

    


## IANA and Windows

https://data.iana.org/time-zones/tz-link.html

https://github.com/unicode-org/cldr/blob/main/common/supplemental/windowsZones.xml

https://secure.jadeworld.com/developer-centre/Jade2020/OnlineDocumentation/content/resources/encyclosys2/jadetimezone_class/ianawindowstimezonemapping.htm

Converting IANA time zone names (e.g., America/New_York) to Windows Registry names (e.g., Eastern Standard Time) requires mapping, as Windows does not use IANA identifiers directly. The most reliable way to do this is using the Unicode CLDR mapping data or specialized libraries. 

1. The Official Mapping Source (CLDR)
Unicode provides the master mapping file, which is updated regularly.

    Source: windowsZones.xml
    How to read: Look for the <mapZone other="..." territory="001" type="..."/> entries.
        type: IANA Time Zone (e.g., Europe/Berlin)
        other: Windows Registry Name (e.g., W. Europe Standard Time) 

2. programmatic Conversion (.NET/C#)
If you are working in a .NET environment, the TimeZoneConverter library is the industry standard for this task. 
csharp

// NuGet: TimeZoneConverter
using TimeZoneConverter;

// IANA to Windows
string windowsZone = TZConvert.IanaToWindows("America/New_York"); // Returns "Eastern Standard Time"

// Windows to IANA
string ianaZone = TZConvert.WindowsToIana("Eastern Standard Time"); // Returns "America/New_York"

Use code with caution.
3. PowerShell Method
You can use .NET capabilities within PowerShell to find the mapping.
powershell

[System.TimeZoneInfo]::FindSystemTimeZoneById("America/New_York").DisplayName

Use code with caution.
4. Direct Registry Location
Once you have the Windows name, the settings are stored here:

    Path: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones 

Key Considerations

    Many-to-One: Multiple IANA zones often map to a single Windows zone (e.g., Europe/Berlin, Europe/Rome, and Europe/Stockholm all map to W. Europe Standard Time).
    Unmappable Zones: Almost all IANA zones can be mapped, but Antarctica/Troll has no equivalent in Windows.
    Use .NET 6+: If you are using .NET 6 or higher, TimeZoneInfo.FindSystemTimeZoneById accepts both IANA and Windows names directly, handling the conversion automatically. 


## GeoCalculator

Check results using

https://aa.usno.navy.mil/data/RS_OneYear

https://theskylive.com/moon-calendar?year=2026

Moon phase algorithm

https://www.celestialprogramming.com/risesetalgorithm.html

https://fcds.cs.put.poznan.pl/MyWeb/Praca/Ubiquitous/LunarPhases.pdf

Javascript

http://hinch.me.uk/riset.html



## MAUI

https://www.codemag.com/Article/2408041/Exploring-.NET-MAUI-Getting-Started

https://blog.ewers-peters.de/are-you-using-dependency-injection-in-your-net-maui-app-yet

https://www.nuget.org/packages/NLog.Targets.MauiLog

https://github.com/EmDe-NJ/Bundled-SQLite-Database-NetMaui/tree/master

https://www.damirscorner.com/blog/posts/20221021-AvoidAsyncCallsInViewmodelConstructors.html

icons fonts

https://fonts.google.com/icons?selected=Material+Symbols+Outlined:settings:FILL@0;wght@400;GRAD@0;opsz@24&icon.size=24&icon.color=%231f1f1f

https://www.reddit.com/r/dotnetMAUI/comments/1g3ir5k/what_do_you_use_for_icons/

info == e88e
settings == e8b8
globe == e64c
plan == ebcc
moon == ef44

back == e5c4

toolbar

https://github.com/dotnet/maui/issues/9240

https://blog.ewers-peters.de/customize-the-title-bar-of-a-maui-app-with-these-simple-steps

https://github.com/dotnet/maui/issues/23201


Date picker

https://medium.com/syncfusion/choosing-the-right-net-maui-picker-date-time-and-lists-made-simple-69d597bf20a3

https://www.stephanarnas.com/posts/maui-date-picker-nullable

https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/datepicker?view=net-maui-10.0


slider

https://stackoverflow.com/questions/73521926/is-there-a-way-to-set-an-interval-or-tick-on-a-slider-in-net-maui



