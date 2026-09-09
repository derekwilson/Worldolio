del bin\Release\net10.0-android\net.derekwilson.worldoliomauipoc*.apk
dotnet clean --configuration Release
dotnet publish -c Release -f net10.0-android -p:AndroidSdkDirectory="D:\AndroidSDK" -p:AndroidPackageFormats=apk
copy bin\Release\net10.0-android\net.derekwilson.worldoliomauipoc-Signed.apk ..\..\..\Support\CurrentBuild\Android
pause