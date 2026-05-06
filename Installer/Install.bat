@echo off
setlocal

echo Installing GCode Preview Handler...

set TARGET="C:\Program Files\GCodePreviewHandler"
if not exist %TARGET% mkdir %TARGET%

copy /Y GCodePreviewHandler.dll %TARGET%
copy /Y GCodePreviewControl.dll %TARGET%

reg import RegisterPreviewHandler.reg

echo Restarting Explorer...
taskkill /f /im explorer.exe >nul
start explorer.exe

echo Done.
pause
