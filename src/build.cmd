REM %WINDIR%\Microsoft.NET\Framework\v2.0.50727\msbuild "%~dp0ML NET20\ML NET20.csproj"
REM %WINDIR%\Microsoft.NET\Framework\v2.0.50727\msbuild "%~dp0ML.Drawing NET20\ML.Drawing NET20.csproj"
REM %WINDIR%\Microsoft.NET\Framework\v2.0.50727\msbuild "%~dp0ML.WinForm NET20\ML.WinForm NET20.csproj
REM %WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild "%~dp0ML NET40\ML NET40.csproj"

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild "MLProject.sln"

REM SET BUILD=Debug
SET BUILD=Release

xcopy "%~dp0ML NET40\bin\%BUILD%\*.dll" "%~dp0TempTest\bin\Debug" /F /R /Y
xcopy "%~dp0ML NET20\bin\%BUILD%\*.dll" "%~dp0TestForNET20\bin\Debug" /F /R /Y
pause