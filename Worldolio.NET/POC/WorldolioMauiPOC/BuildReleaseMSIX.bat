dotnet clean --configuration Release
dotnet publish -c Release -f net8.0-windows10.0.19041.0 /p:GenerateAppxPackageOnBuild=true
copy bin\Release\net8.0-windows10.0.19041.0\win10-x64\AppPackages\WorldolioMauiPOC_0.0.26.3_Test\Worldolio*.MSIX ..\..\..\Support\CurrentBuild\Windows
pause