 # Quandl Excel Add-in for Windows

## Install

1. Download and install file:///C:/Program%20Files%20(x86)/Microsoft%20Visual%20Studio%2014.0/Common7/IDE/Extensions/InstallShield/InstallShieldProject/1033/InstallShield_ult.html
2. Download and install the latest NuGet package at [https://dist.nuget.org/index.html](https://dist.nuget.org/index.html) 
3. Restart `Visual Studio`
4. Right click solution file and select `Manage NuGet Packages for Solution`
5. Select `Nowtonsoft.Json` packages and install the latest stable version
6. Select `Excel-DNA` packages and install the lastest stable version to QuandlFunctions project
7. Delete file `quandl-excel-windows_TemporaryKey.pfx`
8. Go to the project properties for `Quandl.Excel.Addin`
9. Click on signing tab
10. Click `Create Test Certificate` without a password
11. You should now be able to build the project.