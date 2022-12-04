REM @echo off

call :GetWin10SdkDirHelper HKLM\SOFTWARE\Wow6432Node > nul 2>&1
if errorlevel 1 call :GetWin10SdkDirHelper HKCU\SOFTWARE\Wow6432Node > nul 2>&1
if errorlevel 1 call :GetWin10SdkDirHelper HKLM\SOFTWARE > nul 2>&1
if errorlevel 1 call :GetWin10SdkDirHelper HKCU\SOFTWARE > nul 2>&1

setlocal
@REM set PATH="C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x86";%PATH%
set PATH="%WindowsSdkDir%\bin\10.0.22621.0\x86";%PATH%
fxc.exe %*
endlocal
exit /b 0

:GetWin10SdkDirHelper

@REM Get Windows 10 SDK installed folder
for /F "tokens=1,2*" %%i in ('reg query "%1\Microsoft\Microsoft SDKs\Windows\v10.0" /v "InstallationFolder"') DO (
    if "%%i"=="InstallationFolder" (
        SET WindowsSdkDir=%%~k
    )
)