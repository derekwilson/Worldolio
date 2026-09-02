del bin\Release\net8.0-android\net.derekwilson.worldoliomauipoc*.apk
dotnet clean --configuration Release
dotnet publish -c Release -f net8.0-android -p:AndroidPackageFormats=apk
copy bin\Release\net8.0-android\net.derekwilson.worldoliomauipoc-Signed.apk ..\..\..\Support\CurrentBuild\Android
pause