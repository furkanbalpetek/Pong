@echo off
cd /d "%~dp0"
for /f "delims=" %%i in ('git log -1 --pretty^=%%s') do set "LAST=%%i"
echo Eski not: "%LAST%"
echo "%LAST%" | findstr /i /r "\<rowser" >nul
if errorlevel 1 goto nochange
set "NEW=%LAST:rowser=Browser%"
echo Yeni not: "%NEW%"
git commit --amend -m "%NEW%"
git push --force-with-lease origin main
echo.
echo Duzeltildi.
pause
exit /b
:nochange
echo Son commit notunda hatali "rowser" kelimesi yok, hicbir sey degistirilmedi.
pause
