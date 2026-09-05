# Build Area

This is where the build scripts will produce release artifacts. It also contains scripts to work with those artifacts.

## Android

The APK will be called `net.derekwilson.worldoliomauipoc-Signed.apk`

- `InstallReleaseApk.bat` will install the APK on an attached device
- `ShowApkFingerprint.bat` will display the signing key fingerprint for the passed APK filename the key should match the one below

```
\AndroidSDK\build-tools\34.0.0\apksigner verify --print-certs net.derekwilson.worldoliomauipoc-Signed.apk
Signer #1 certificate DN: CN=Andrew and Derek
Signer #1 certificate SHA-256 digest: 561d89ad72b75fac8ff990d72f3c4ee6bc4f0805ccac070fa274e04c4ae914af
Signer #1 certificate SHA-1 digest: 01b9a03a8eeb6076b4b6bb355a1180966eab682c
Signer #1 certificate MD5 digest: 311a42e615edb0c1cc5d8c700f52a10c
```

## Windows

The MSIX will be called `WorldolioMauiPOC_<version>_x64.msix`

- `worldolio_windows.cer` the public key for the MSIX

Before you can  install the MSIX on a Windows machine the public key CER file must be installed into either the `Root` or `Trusted People` store. Do this by double clicking on the CER file and then Selecting Install. You only need to install the key the first time.


