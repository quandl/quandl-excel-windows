# Quandl Excel Add-in for Windows

The Quandl Excel Add-In allows you to search through, find and download any of Quandl's millions of datasets directly from within Microsoft Excel. It's completely free; usage is unlimited and unrestricted. Currently this Add-in is limited to windows only as it uses features and functions which are only available on the windows version of excel.

## Development

A few things that will make your excel development experience much easier:

* Excel is single threaded
* Use Async tasks and don't block with long running code. This will block the Excel UI due to it being single threaded.
* When making calls to excel via ExcelDNA thing of Excel as the `server` and our ExcelDNA app as the `client`. Design your application as if you are making `requests` of excel which it may or may not fulfill. Also not that the Excel `server` can be busy (overloaded) due to its single threaded nature and you may need to wait and retry your call later to fulfill it.

### Setup

1. Download and install InstallSheild Limited Edition [here](http://learn.flexerasoftware.com/content/IS-EVAL-InstallShield-Limited-Edition-Visual-Studio)
2. Right click solution file and select `Manage NuGet Packages for Solution`
(If you don't have NuGet, please install it at [https://dist.nuget.org/index.html](https://dist.nuget.org/index.html))
3. Select `Newtonsoft.Json` packages and install the latest stable version
4. Select `Excel-DNA` packages and install the lastest stable version to QuandlFunctions project
5. Go to the project properties for `Quandl.Excel.Addin`
6. Click on signing tab
7. Click `Create Test Certificate` without a password
8. Do steps 5-7 for `Quandleild.Excel.Console`
9. Go to the project properties for `Quandl.Excel.UDF.Functions`
10. Under `Debug` change the `Start Action` from `Start Project` to `Start External Program`
11. Fill in the path to your chosen version of Excel in the Textbox
12. Under `Start Options` in the `Command Line Arguments` text field enter `Quandl.Excel.UDF.Functions-AddIn.xll`
13. You should now be able to build the project.

## Building a Release package

Following steps will create a setup package which works for both Microsoft Excel 32 bit and 64 bit.

### Preparation

1. Follow the instructions list in `Development` section above to setup the project and its basic dependencies.
2. Copy this file [Microsoft .NET Framework 4.6.1 Web.prq](Microsoft .NET Framework 4.6.1 Web.prq) to folder C:\Program Files (x86)\InstallShield\2015LE\SetupPrerequisites

### Releasing

1. Ensure the setup project is signed `Quandl.Excel.Addin.Setup -> 6 Prepare for Release => Releases => SingleImage => Signing`
  * See [SIGNING](SIGNING.md)
2. Navigate to `Quandl.Excel.Addin.Setup -> 1 Organize Your Setup => General Information`
  1. Change the product code (use the helper - `{...}`)
  2. Bump the version number.
    * Be sure to leave the upgrade code untouched.
3. Navigate to the `Quandl.Excel.Addin -> Properties => Publish` and update the version to match the setup version.
4. Navigate to the `Quandl.Shared.Modules -> Utilities => ReleaseVersion` and update the version to match the setup version.
5. Switch your `Run Mode` to `release` instead of `debug`
6. Right click solution file and select `Rebuild Solution`
7. Select the `Quandl.Excel.Addin.Setup` project and in the topbar `InstallShield LE` menu select `Open release folder` to find your setup.exe file.

Things to note:

* UnRegisterAddin must have code `1501` in the `.isl` file.
* Be sure to bump the version AND change your product code number under `Organize Your Setup` => `General Information`. This is necessary for a seemless upgrade.
* Allow of our dependencies have been listed as `web` dependencies to keep our installer small. 
  * Should you need to install them locally you can do that navigating to `Quandl.Excel.Addin.Setup -> 2 Specify Application Data => Redistributables`. You will need to do this in Visual Studio as an admin.
* we are using [Markdown.XAML](https://github.com/theunrepentantgeek/Markdown.XAML) to generate the flowdocument from the github markup. For more info check out the github page.

## Unit testing

See [Unit Testing Guide](UNIT_TEST_GUIDE.md)

## FAQ

For a list of excel exceptions and how to debug them please see: [Errors](./ERRORS.md)

### The VSTO Add-in and or UDF excel plugin is listed but not displaying/activating

Excel seems to a have a bug where even when you close all its windows it can leave the main process running in the background. This seems to be for quick preview reasons (when you click an excel file in explorer). When this happens excel unloads all its add-ins to save memory. However this also means that it won't reload them until next time its started properly. To reload the add-in you must forcibly close any remaing excel instances from the task manager `details` tab.

### My UDF function was running along great but then appears to have stopped updating

This could be a number of things but generally means that our implementation has run into one of the following problems:

* Unhandled excel exception - Excel has many mysterious exceptions that result in COMException errors. If left unhandled they can crash excel or stop the running UDF.
* Threading deadlock - This can happen if we use our threads in a non thread safe way. Basically we are not handling a specific case properly.  
* Excel request deadlock - This can occur when excel is busy and we try to make another request to it from a different thread.
* Unhandled server error response - Our server is having issues and after a few retries our code simply gives up.

### InstallShield Limited Edition is not displaying the Redistributables

Double check your installshield limited edition settings. Sometimes it will point to the wrong redistributables folder even after re-installation. In most cases it should be set to `C:\Program Files (x86)\InstallShield\2015LE\SetupPrerequisites`

## License

See [LICENCE](LICENCE.md) file for licence rights and limitations (MIT)
 
