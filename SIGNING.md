###Signing

1. Download and place the a certificate in the root of `Quandl.Excel.Addin` and `Quandl.Excel.Console`.
  * The output of each project needs to be signed individually
  * Additionally the installer needs to be signed as well
  * If your Cert has been password protected it will need the decryption key to be used. See [Password Protect Digital Certificate](#password-protect-digital-certificate)
2. Add the path to the certificate and the password to the installer in the `Prepare For Release -> Releases -> SingleImage -> Signing` menu.

## Password Protect Digital Certificate

1. Right click on each project with a digital certificate
2. Select Properties -> Signing -> Select From File
3. Select the Digital Certificate
4. Enter password (found in lastpass) and Save