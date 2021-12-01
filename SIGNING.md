# Signing

1. Open powershell as an administrator, Search for PowerShell in Applications
1. Navigate to code root directory
1. Set the execution policy to allow unsigned scripts to run with `set-executionpolicy unrestricted`
1. Run `.\SignQuandlExcelInstaller.ps1`
1. Follow the instructions to sign the dlls and the installer itself (there is a break in the middle where you need to build the installer again)
  * Right click Quandl.Excel.AddinSetup and choose Rebuild
  * If you have issues writing to a folder check for MSBuild.exe in task manager and end the task