## Proof of concept apps

This folder contains the code Worldolio POCs.

- `WorldolioDataChecker` is a data checker tool that checks that the Worldolio Sqlite DB has been build correctly and that the TZ IANA names are correct
- `WorldolioPOC` is the proof of concept code using console output and CLI making use of the Worldolio domain data 
- `MauiPOC` is the proof of concept code for MAUI using simple empty screens to check the UI
- `WorldolioMauiPOC` is the proof of concept code for MAUI making use of the Worldolio domain data


### WorldolioDataChecker

VS2022 / VS2026
Targets .NET 8
CLI

Example output

```
WorldolioDataChecker v1.0.0.3
Running on .NET CLR: 8.0.31
Worldolio INFO WorldolioDataChecker, v1.0.0.3, Running on .NET CLR: 8.0.31
Worldolio INFO DI init OK
DriveSide 0 Unknown
DriveSide 1 Left
DriveSide 2 Right
DriveSide count = 3
Country AF, AFG, Afghanistan, Right, Cities = 1
Country AL, ALB, Albania, Right, Cities = 1

...

Country ZM, ZMB, Zambia, Left, Cities = 1
Country ZW, ZWE, Zimbabwe, Left, Cities = 1
Countires count = 221

City 1, Aberdeen, United Kingdom, Pos 2.07W, 57.13N Drives Left
   TZ Europe/London, 12:58 PM, 25 Oct 2026,28 Mar 2027
City 486, Abidjan, Cote d'Ivoire, Pos 4.04W, 5.32N Drives Right
   TZ Africa/Abidjan, 11:58 AM, No DST

...

City 472, Zaragoza, Spain, Pos 0.89W, 41.65N Drives Right
   TZ Europe/Madrid, 1:58 PM, 25 Oct 2026,28 Mar 2027
City 473, Zurich, Switzerland, Pos 8.32E, 47.22N Drives Right
   TZ Europe/Zurich, 1:58 PM, 25 Oct 2026,28 Mar 2027
Cities count = 519, invalid TZ = 0
Country NZ, NZL, New Zealand, Left, Cities = 4
     City 33, Auckland, New Zealand
     City 475, Christchurch, New Zealand
     City 478, Palmerston North, New Zealand
     City 458, Wellington, New Zealand
SQLite version: 3.53.3, Schema Version: 1
18/05/2026 21:17:19, Schema Init v3.0 - start SQLIte v3.53.0
18/05/2026 21:17:20, Schema Init - end
18/05/2026 21:17:20, Schema Init Data Load v3.0 - start
18/05/2026 21:17:21, City count = 519
18/05/2026 21:17:21, Country count = 221
18/05/2026 21:17:21, Drive Side count = 3
18/05/2026 21:17:21, Schema Init Data Load v3.0 - end
SRA count = 7
```

If any data fails to load then an error will be reported


### WorldolioPOC

VS2022 / VS2026
Targets .NET 8
CLI

The output depends on what is being tested


### MauiPOC

VS2026
Targets .NET 10
Android and Windows

Screenshots in the support folder

### WorldolioMauiPOC

VS2026
Targets .NET 10
Android and Windows

Screenshots in the support folder
