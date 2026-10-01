## Worldolio applications

This folder contains the code for the Worldolio applications

- `WorldolioCLI` is a CLI implementation of Worldolio


### WorldolioCLI

VS2022 / VS2026
Targets .NET 8
CLI

Should work on any .NET 8 platform: Windows, Mac, Linux. The `worldolio.sqlite` database must be present in the current directory or an error will be thrown.

Usage

```
WorldolioCLI v1.0.0.6
Running on .NET CLR: 8.0.31
Microsoft Windows 10.0.26200, Framework: .NET 8.0.31, OS: X64, Processor: X64
TZ Database: 2026b
Database: Schema: 1, 18/05/2026 21:17:19
Usage: WorldolioCLI <command> <param> <date> <time>
Where
  <command> = command to execute. find | citylist | countrylist
  <param> = depends on <command>
    find = a string to look for in city or country name
    citylist = list of numeric city ids to display
    countrylist = list of country ids to display
  <date> = date to use in yyyy-mm-dd format, default is today
  <time> = time to use in hh:mm 24 hour format, default is the current time
```

Finding a city: `WorldolioCLI.exe find "new"`

```
WorldolioCLI v1.0.0.6
Running on .NET CLR: 8.0.31
Microsoft Windows 10.0.26200, Framework: .NET 8.0.31, OS: X64, Processor: X64
TZ Database: 2026b
Database: Schema: 1, 18/05/2026 21:17:19
3 cities match with 'new'
New Orleans; LA, United States, ID 291
New York; NY, United States, ID 292
Newark; NJ, United States, ID 293
3 countries match with 'new'
NC, NCL, New Caledonia
  Noumea,   ID 500
NZ, NZL, New Zealand
  Auckland,   ID 33
  Christchurch,   ID 475
  Palmerston North,   ID 478
  Wellington,   ID 458
PG, PNG, Papua New Guinea
  Port Moresby,   ID 329
Hit count = 6
```

Using the city IDs displayed by the `find` command

Displaying the city grid: `WorldolioCLI.exe citylist 458,429,252,477`

```
WorldolioCLI v1.0.0.6
Running on .NET CLR: 8.0.31
Microsoft Windows 10.0.26200, Framework: .NET 8.0.31, OS: X64, Processor: X64
TZ Database: 2026b
Database: Schema: 1, 18/05/2026 21:17:19
City Grid = 458, [458,429,252,477]
Date Time = 01 Oct 2026, 13:19
City 458, Wellington, New Zealand, Pos 174.77E, 41.26S Drives Left
   Thu 1:19 PM
   No offset, DST 04 Apr 2027,26 Sept 2027, TZ Pacific/Auckland
   Nearby cities count = 3
   Sunrise: 6:55 AM, Sunset: 7:25 PM, Noon: 1:10 PM
   Moonrise: Fri 02, 1:01 AM, Moonset: Fri 02, 9:45 AM
City 429, Tokyo, Japan, Pos 139.76E, 35.67N Drives Left
   Thu 9:19 AM
   4:00 behind, DST No DST, TZ Asia/Tokyo
   Nearby cities count = 13
   Sunrise: 5:34 AM, Sunset: 5:26 PM, Noon: 11:30 AM
   Moonrise: Thu 01, 8:07 PM, Moonset: Thu 01, 10:30 AM
City 252, Manchester, United Kingdom, Pos 2.23W, 53.47N Drives Left
   Thu 1:19 AM
   12:00 behind, DST 25 Oct 2026,28 Mar 2027, TZ Europe/London
   Nearby cities count = 42
   Sunrise: 7:08 AM, Sunset: 6:48 PM, Noon: 12:58 PM
   Moonrise: Thu 01, 8:22 PM, Moonset: Thu 01, 1:58 PM
City 477, Bergerac, France, Pos 0.50E, 44.85N Drives Right
   Thu 2:19 AM
   11:00 behind, DST 25 Oct 2026,28 Mar 2027, TZ Europe/Paris
   Nearby cities count = 50
   Sunrise: 7:54 AM, Sunset: 7:40 PM, Noon: 1:47 PM
   Moonrise: Thu 01, 10:07 PM, Moonset: Thu 01, 1:52 PM
Cities count = 4, invalid TZ = 0
```

Or to specify a date and time `WorldolioCLI.exe citylist 458,429,252,477 2026-11-01 13:00`


