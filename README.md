# Quandl Excel Add-in for Windows

The Quandl Excel Add-In allows you to search through, find and download any of Quandl's millions of datasets directly from within Microsoft Excel. It's completely free; usage is unlimited and unrestricted. Currently this Add-in is limited to windows only as it uses features and functions which are only available on the windows version of excel.

## Development

A few things that will make your excel development experience much easier:

* Excel is single threaded
* Use Async tasks and don't block with long running code. This will block the Excel UI due to it being single threaded.
* When making calls to excel via ExcelDNA thing of Excel as the `server` and our ExcelDNA app as the `client`. Design your application as if you are making `requests` of excel which it may or may not fulfill. Also not that the Excel `server` can be busy (overloaded) due to its single threaded nature and you may need to wait and retry your call later to fulfill it.

### Setup

1. Install Add-in Express for Office and .NET from [https://www.add-in-express.com/downloads/adxnet.php]. Any edition will work. Note that trial version is not available, you need to purchase your license.
1. Install WiX toolset from http://wixtoolset.org/.
1. Right click solution file and select `Restore NuGet Packages`
(If you don't have NuGet, please install it at [https://dist.nuget.org/index.html](https://dist.nuget.org/index.html))
1. Make sure that you have `ildasm` tool from Microsoft SDK installed. You need it to sign assemblies with strong name (see the next step). The solution has been tested with SDK 10.0A
1. Modify `3rdparty\makesn.cmd` to change path to SDK folder (for `sn.exe`, `ildasm.exe` tools) and possibly to Microsoft.NET folder (for `ilasm.exe` tool), if appropriate.
2. Run `3rdparty\makesn.cmd`. It should create strongly signed versions of `Markdown.Xaml.dll`, `octokit.dll`, `SharpRaven.dll` and `Syroot.Windows.IO.KnownFolders.dll`.
1. Go to the project properties for `Quandl.Excel.Addin`
1. Click on signing tab
1. Click `Create Test Certificate` without a password
1. Repeat 3 last steps for `Quandl.Excel.Console`
1. Go to the project properties for `Quandl.Excel.UDF.Functions`
1. Under `Debug` change the `Start Action` from `Start Project` to `Start External Program`
1. Fill in the path to your chosen version of Excel in the Textbox
1. Under `Start Options` in the `Command Line Arguments` text field enter `Quandl.Excel.UDF.Functions-AddIn.xll`
1. Whitelist your development plugin in Excel 
	1. Open Excel
	1. Click `File -> Options -> Truste Center`
	1. Click `Trust Center Settings`
	1. Click `Trusted Locations -> Add New Location`
	1. Enter the root directory of your project, example `C:\Users\Developer\Projects\quandl-excel-windows\`
	1. Ensure `Subfolders of this location are also trusted` is checked
	1. Click OK    
1. You should now be able to build the project.

## Building a Release package

Following steps will create a setup package which works for both Microsoft Excel 32 bit and 64 bit.


### Releasing

1. Ensure the setup project is signed `Quandl.Excel.Addin.Setup -> 6 Prepare for Release => Releases => SingleImage => Signing`
  * See [SIGNING](SIGNING.md)
1. Navigate to `Quandl.Excel.Addin.Setup -> Product.wxs`
  1. Change the product code (use the helper - `{...}`)
  1. Bump the version number.
    * Be sure to leave the upgrade code untouched.
1. Navigate to the `Quandl.Excel.Addin -> Properties => Publish` and update the version to match the setup version.
1. Navigate to the `Quandl.Shared.Modules -> Utilities => ReleaseVersion` and update the version to match the setup version.
1. Switch your `Run Mode` to `release` instead of `debug`
1. Right click solution file and select `Rebuild Solution`

Things to note:

* You might need to kill processs `msbuild.exe` to recompile the solution. 
* ProductId is set to `*` in `Quandl.Excel.AddinSetup -> Product.wsx`
* we are using [Markdown.XAML](https://github.com/theunrepentantgeek/Markdown.XAML) to generate the flowdocument from the github markup. For more info check out the github page.
* When testing, if your plugin does not appear in Excel, check that it was not added to the `Disabled Items` list.  To check:
	* Open Excel
	* Click `File -> Options -> Add-Ins`
	* Under the `Manage` dropdown, select `Disabled Items` and click `GO`
	* Enable any instance of the Quandl Add-In that appear there 

## Unit testing

See [Unit Testing Guide](UNIT_TEST_GUIDE.md)

## FAQ

For a list of excel exceptions and how to debug them please see: [Errors](./ERRORS.md)

### My UDF function was running along great but then appears to have stopped updating

This could be a number of things but generally means that our implementation has run into one of the following problems:

* Unhandled excel exception - Excel has many mysterious exceptions that result in COMException errors. If left unhandled they can crash excel or stop the running UDF.
* Threading deadlock - This can happen if we use our threads in a non thread safe way. Basically we are not handling a specific case properly.  
* Excel request deadlock - This can occur when excel is busy and we try to make another request to it from a different thread.
* Unhandled server error response - Our server is having issues and after a few retries our code simply gives up.

## License

See [LICENCE](LICENCE.md) file for licence rights and limitations (MIT)
 
