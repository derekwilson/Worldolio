dotnet clean --configuration Release
dotnet publish -c Release -f net8.0-windows10.0.19041.0 /p:GenerateAppxPackageOnBuild=true
rem copy bin\Release\net8.0-windows10.0.19041.0\win10-x64\AppPackages\WorldolioMauiPOC_0.0.26.3_Test\Worldolio*.MSIX ..\..\..\Support\CurrentBuild\Windows
rem we cannot do wildcard copy but we can do a wildcard cd, but we need to ensure there is only one dir by doing a clean above
rem pushd bin\Release\net8.0-windows10.0.19041.0\win10-x64\AppPackages\Worldolio*
rem copy Worldolio*.MSIX ..\..\..\..\..\..\..\..\..\Support\CurrentBuild\Windows
rem popd
rem but this is better
FOR /d %%d in (bin\Release\net8.0-windows10.0.19041.0\win10-x64\AppPackages\Worldolio*) DO (
    copy %%d\Worldolio*.MSIX ..\..\..\Support\CurrentBuild\Windows
)
pause