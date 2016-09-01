# Quandl Excel Add-in for Windows

> I wanna be the very best
> Like no one ever was

The Quandl Excel Add-In allows you to search through, find and download any of Quandl's millions of datasets directly from within Microsoft Excel. It's completely free; usage is unlimited and unrestricted. Currently this Add-in is limited to windows only as it uses features and functions which are only available on the windows version of excel.

## Install

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
2. Install the key `QuandlDigitCertCodeSign.pfx` from lastpass to the projects missing it.
  * The output of each project needs to be signed individually
  * Additionally the installer needs to be signed as well
3. Download the dependencies under `Quandl.Excel.Addin.Setup -> 2 Specify Application Data => Redistributables`. You will need to do this as an admin.
4. Ensure your project QuandlDigitCertCodeSign.pfx files have the protected password. See [Password Protect Digital Certificate](#password-protect-digital-certificate)
5. Restart into non-admin mode
6. Ensure the setup project is signed `Quandl.Excel.Addin.Setup -> 6 Prepare for Release => Releases => Signing`
7. Change the product code (use the helper) and bump the version number.
  * Be sure to leave the upgrade code untouched.
8. Right click solution file and select `Rebuild Solution`
9. $PROJECT_ROOT_FOLDER\quandl-excel-windows \QuandlExcelAddinSetup\Quandl.Excel.Addin.Setup\Express\SingleImage\DiskImages\DISK1\setup.exe is the setup package

Things to note:

* UnRegisterAddin must have code `1501` in the `.isl` file.
* Be sure to bump the version AND change your product code number under `Organize Your Setup` => `General Information`. This is necessary for a seemless upgrade.

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

### Unit testing
- see [Unit Testing Guide](UNIT_TEST_GUIDE.md)

## FAQ

For a list of excel COM exceptions and what they mean please see: [Errors](./ERRORS.md)

## License

MIT License
 
