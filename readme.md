# Worldolio v3

The original Worldolio was written for .NET Framework and only worked on Windows desktop. It used the TimeZone data built in to Windows.

This is Worldolio rewritten for .NET. The CLI versions should work on Windows, Mac and Linux. The UI version currently uses MAUI to run on Windows and Android. It uses IANA TimeZone data.

## Objectives

1. Runs on .NET Core, no dependency on Windows or .NET Framework
1. Move from Windows registry TZ info to IANA, either directly or via NodaTime
1. Easy to maintain and update data, as TZs change. Probably build and use a SQL DB (SQLite?)
1. UI should run on Windows and Android so maybe consider MAUI or maybe WinForm and .NET/Mono/Xamarin
1. Off-line use cases are a priority (as there are many online versions like TimeAndDate), so remove weather forecast
1. Planning a common time across multiple cities is the main payback use case

The repo is broken up into the following areas

- `Support` Holds the support files for the app
- `Worldolio.NET` hold the .NET source code for the applications and assemblies


