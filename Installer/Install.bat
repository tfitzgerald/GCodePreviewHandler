@echo off
setlocal

echo Installing GCode Preview Handler...

set TARGET="C:\Program Files\GCodePreviewHandler"
mkdir %TARGET% >nul 2>&1

echo Copying DLLs...
copy /Y GCodePreviewHandler.dll %TARGET%
copy /Y GCodePreviewControl.dll %TARGET%

echo Importing registry entries...
reg import RegisterPreviewHandler.reg

echo Restarting Explorer...
taskkill /IM explorer.exe /F
start explorer.exe

echo Done.
pause
