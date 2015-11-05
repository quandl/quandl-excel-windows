 # Quandl Excel Add-in for Windows

## Install

1. Download and install file:///C:/Program%20Files%20(x86)/Microsoft%20Visual%20Studio%2014.0/Common7/IDE/Extensions/InstallShield/InstallShieldProject/1033/InstallShield_ult.html
 * Open Solution -> right click solution -> add -> new project -> other project type -> InstallShield
 * Walk through the steps on the side to download and install InstallShield (Make note of your install key)
3. Restart `Visual Studio` and enter in your install key
4. Right click solution file and select `Manage NuGet Packages for Solution`
(If you don't have NuGet, please install it at [https://dist.nuget.org/index.html](https://dist.nuget.org/index.html))
5. Select `Nowtonsoft.Json` packages and install the latest stable version
6. Select `Excel-DNA` packages and install the lastest stable version to QuandlFunctions project
7. Delete file `quandl-excel-windows_TemporaryKey.pfx`
8. Go to the project properties for `Quandl.Excel.Addin`
9. Click on signing tab
10. Click `Create Test Certificate` without a password
11. You should now be able to build the project.