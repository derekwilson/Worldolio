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

## UI - using MAUI

Done

1. Get logging to work using NLog
1. Get Dependency Injection to work
1. Get exception handling and logging to work
1. Deploy sqlite DB and access it
1. different package names for debug and release
1. Navigate to a n about screen and back
1. Get tabs to work
1. Display a city grid
1. Implement time to update times in the grid
1. Implement date and time picker for plan tab

Todo

1. use TabbedPage with embedded ContentPage and On<Microsoft.Maui.Controls.PlatformConfiguration.Android>.SetIsSwipePagingEnabled(true)
1. enable FontImageSource to be bound to a global static class
1. rework tabbar partial view to be pure code as the XAML does not really do anything
1. get ripple to work on ImageButton on android
1. auto size font when window changes size
1. get crash reporting to work
1. get analytics to work
1. custom selection lists
1. persistent settings
1. display build type debug/release and build time or git id on about form
1. build a release app for windows
1. build a release app for android
1. show when an item is in daylight
1. select an item in a CollectionView and launch a details view
1. refresh the page when returning from a view
1.
1.


