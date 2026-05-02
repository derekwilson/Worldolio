# Worldolio v3

Worldolio rewritten

## Objectives

1. Runs on .NET Core, no dependency on Windows or .NET Framework
1. Move from Windows registry TZ info to IANA, either directly or via NodaTime
1. Easy to maintain and update data, as TZs change. Probably build and use a SQL DB (SQLite?)
1. UI should run on Windows and Android so maybe consider MAUI or maybe WinForm and .NET/Mono/Xamarin
1. Off-line use cases are a priority (as there are many online versions like TimeAndDate), so remove weather forecast
1. Planning a common time across multiple cities is the main payback use case

