###Signing

1. Download and place the key `QuandlDigitCertCodeSign.pfx` from lastpass in the root of the projects missing it.
  * The output of each project needs to be signed individually
  * Additionally the installer needs to be signed as well
  * The QuandlDigitCertCodeSign.pfx file has been password protected and will need the decryption key to be used. See [Password Protect Digital Certificate](#password-protect-digital-certificate)
  
### Password Protect Digital Certificate

1. Right click on each project with a digital certificate
2. Select Properties -> Signing -> Select From File
3. Select the Digital Certificate
4. Enter password (found in lastpass) and Save