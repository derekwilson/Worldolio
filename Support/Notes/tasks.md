# Steps to get to MVP

The plan

## Foundations

1. Use NLog config from SolarTools
1. MixPanel for analytics
1. MS Dependency injection, see PodcastUtilities
1. Moq for testing

## Data and domain objects - WOData

1. copy cities.csv and countries.csv
1. convert windows TZ ID to IANA
1. update cities.csv to use IANA
1. load csv into sqlite db
1. write DB tester to check IANA ids are OK in NodaTime
1. port domain objects - City and Country
1. write DB repository
1. add in geocalculator

## Controls - WOControls

1. MAUI ??

