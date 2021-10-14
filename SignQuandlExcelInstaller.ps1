Set-Location 'C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x86'

$code_dir=  Read-Host -Prompt "Please enter the code's root directory"
$cert_location = Read-Host -Prompt "Please enter the absolute path to the Certificate"
$cert_password = Read-Host -Prompt "Please enter the Cert's password"

.\signtool.exe  sign /tr http://timestamp.digicert.com /td sha256 /fd sha256 /v /f $cert_location /p $cert_password "$code_dir\Quandl.Excel.Addin\bin\Release\Quandl.Excel.Addin.dll" "$code_dir\Quandl.Excel.UDF.Functions\bin\Release\Quandl.Excel.UDF.Functions.dll" 

if (-NOT  (0 -eq $LastExitCode)) {
  throw "signing dlls failed aborting"
}

Read-Host -Prompt "Please rebuild the installer before pressing Enter"

.\signtool.exe  sign /tr http://timestamp.digicert.com /td sha256 /fd sha256 /v /f $cert_location /p $cert_password "$code_dir\Quandl.Excel.AddinSetup\4.0.0\bin\Release\en-US\NasdaqDataLink.Excel.AddinSetup.msi"
