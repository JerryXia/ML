%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild "MLProject.sln"


REM SET BUILD=Debug
SET BUILD=Release

xcopy "%~dp0ML NET20\bin\%BUILD%\*.dll" "%~dp0TempTest\bin\Debug" /F /R /Y
pause