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

1. Follow the instructions list in `Development` section above, setup the project
2. Install the key `QuandlDigitCertCodeSign.pfx` from lastpass to the projects missing it
3. Download the dependencies under `Quandl.Excel.Addin.Setup -> 2 Specify Application Data => Redistributables`. You will need to do this as an admin.
4. Ensure your project QuandlDigitCertCodeSign.pfx files have the protected password. See [Password Protect Digital Certificate](#password-protect-digital-certificate)
5. Restart into non-admin mode
6. Right click solution file and select `Rebuild Solution`
7. $PROJECT_ROOT_FOLDER\quandl-excel-windows \QuandlExcelAddinSetup\Quandl.Excel.Addin.Setup\Express\SingleImage\DiskImages\DISK1\setup.exe is the setup package

Things to note:

* UnRegisterAddin must have code `1501` in the `.isl` file.

### Password Protect Digital Certificate

1. Right click on each project with a digital certificate
2. Select Properties -> Signing -> Select From File
3. Select the Digital Certificate
4. Enter password (found in lastpass) and Save

### Sign installation package
1. Download  `QuandlDigitCertCodeSign.pfx` from lastpass to your local windows folder 
2. Use windows application `certmgr.msc` to to import QuandlDigitCertCodeSign.pfx to your local certification store
3. `cd  $PROJECT_ROOT_FOLDER\quandl-excel-windows\QuandlExcelAddinSetup\Quandl.Excel.Addin.Setup\Express\SingleImage\DiskImages\DISK1`
4. run this command to sign your setup package `SignTool sign /n "Quandl Inc." setup.exe`

## License

MIT License
 
