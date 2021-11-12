"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\sn.exe" -k keyPair.snk
set PATH_ILDASM="%ProgramFiles(x86)%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\ildasm.exe"
set PATH_ILASM=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\ilasm.exe
set PATH_AIE="%ProgramFiles(x86)%\Add-in Express\Add-in Express for .NET\Redistributables"
call :runsn ..\lib\Markdown.Xaml.dll Markdown.Xaml
call :runsn ..\packages\Octokit.0.24.0\lib\net45\Octokit.dll octokit
call :runsn ..\packages\Syroot.Windows.IO.KnownFolders.1.0.2\lib\net40\Syroot.Windows.IO.KnownFolders.dll Syroot.Windows.IO.KnownFolders
call :cpdll ..\Quandl.Excel.Addin\Loader
call :cpexe ..\Quandl.Excel.AddinSetup\4.0.0\Resources
EXIT /B %ERRORLEVEL%

:runsn
del %2%.*
%PATH_ILDASM% "%1"  /out=%2%.il
%PATH_ILASM% %2%.il /dll /key=keyPair.snk
del %2%.il
del %2%.res

:cpdll
copy %PATH_AIE%\adxloader64.dll %1%
copy %PATH_AIE%\adxloader.dll %1%
EXIT /B 0

:cpexe
copy %PATH_AIE%\vdwtool.exe %1%
EXIT /B 0
