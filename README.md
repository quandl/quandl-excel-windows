# Quandl Excel Add-in for Windows

The Quandl Excel Add-In allows you to search through, find and download any of Quandl's millions of datasets directly from within Microsoft Excel. It's completely free; usage is unlimited and unrestricted. Currently this Add-in is limited to windows only as it uses features and functions which are only available on the windows version of excel.

## Install

### End User

http://www.quandl.com/help/excel

### Development

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

### Build installation package

1. Using git tool and check out this project in folder C:\Users\Developer\code\quandl-excel-windows
2. Follow the instructions list in `Development` section above, setup the project
3. Right click solution file and select `Rebuild Solution`
4. C:\Users\Developer\code\quandl-excel-windows \QuandlExcelAddinSetup\Quandl.Excel.Addin.Setup\Express\SingleImage\DiskImages\DISK1\setup.exe is the setup package

### Sign installation package
1. `cd  C:\Users\Developer\code\quandl-excel-windows\QuandlExcelAddinSetup\Quandl.Excel.Addin.Setup\Express\SingleImage\DiskImages\DISK1`
2. run command `SignTool sign /n "Quandl Inc.` setup.exe

## License

MIT License
 
