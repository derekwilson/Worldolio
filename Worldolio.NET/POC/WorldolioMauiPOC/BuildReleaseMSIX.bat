dotnet clean --configuration Release
dotnet publish -c Release -f net8.0-windows10.0.19041.0 /p:GenerateAppxPackageOnBuild=true
rem copy bin\Release\net8.0-windows10.0.19041.0\win10-x64\AppPackages\WorldolioMauiPOC_0.0.26.3_Test\Worldolio*.MSIX ..\..\..\Support\CurrentBuild\Windows
rem we cannot do wildcard copy but we can do a wildcard cd, but we need to ensure there is only one dir by doing a clean above
pushd bin\Release\net8.0-windows10.0.19041.0\win10-x64\AppPackages\Worldolio*
copy Worldolio*.MSIX ..\..\..\..\..\..\..\..\..\Support\CurrentBuild\Windows
popd
pause