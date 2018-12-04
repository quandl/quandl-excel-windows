# Signing

1. Open powershell as an administrator
2. Navigate to code root directory
3. Set the execution policy to allow unsigned scripts to run with `set-executionpolicy unrestricted`
4. Run `SignQuandlExcelInstaller.ps1`
5. Follow the instructions to sign the dlls and the installer itself (there is a break in the middle where you need to build the installer again)