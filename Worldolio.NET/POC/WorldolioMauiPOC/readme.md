## MAUI POC for Worldolio

This folder contains the code for the POC.


### WorldolioMauiPOC


Is built using VS2022


#### Building the Release Build

Note: You need to have the folder `Worldolio\LocalOnly`. Its not in the repo you will need to get it from one of the team, if you dont have it then you can still build the debug builds but you cannot sign the release builds.

##### Building a release APK

If you are intending to deploy the app using Amazon App Store or by having the user download it from GitHub then you must build an APK, as phones cannot install AAB's (thanks Google). The APK will be signed using the key in `LocalOnly`, this will be the app signing key as the user will install the APK directly, the play store is not involved. 

Note: Sometimes when the `BuildReleaseAPK.bat` ,is first run it will actually produce an APK named for the `debug` configuration like this `net.derekwilson.worldoliomauipoc-debug-Signed.apk`. Not sure if this is an artefact of having the IDE running at the same time but running it for a second time seems to fix the issue. 

1. In VS open `WorldolioMauiPOC.csproj` enter the correct `ApplicationVersion` and `ApplicationDisplayVersion`
1. Open a developer command prompt for VS2022
1. Goto `Worldolio.NET\POC\WorldolioMauiPOC`
1. Run `BuildReleaseAPK.bat`
1. The `apk` will be copied to `Support\CurrentBuild\Android` and will be called `net.derekwilson.worldoliomauipoc-Signed.apk`
1. Goto `Support\CurrentBuild\Android`
1. Connect a test device
1. Run `InstallReleaseApk.bat`


In `Support\CurrentBuild\Android` if you run `ShowApkFingerprint.bat net.derekwilson.worldoliomauipoc-Signed.apk` you should see

```
Signer #1 certificate DN: CN=Andrew and Derek
Signer #1 certificate SHA-256 digest: 561d89ad72b75fac8ff990d72f3c4ee6bc4f0805ccac070fa274e04c4ae914af
Signer #1 certificate SHA-1 digest: 01b9a03a8eeb6076b4b6bb355a1180966eab682c
Signer #1 certificate MD5 digest: 311a42e615edb0c1cc5d8c700f52a10c
```
