@echo off
setlocal
cd /d "%~dp0"
set "PATH=%PATH%;C:\Program Files\Git\cmd"

echo === Degisiklikler ===
git status --short
echo.
set /p MSG=Bu kayit icin kisa bir not yaz (bos birakirsan "Guncelleme" olur):
if "%MSG%"=="" set "MSG=Guncelleme"

git add .
git commit -m "%MSG%"
git push

echo.
echo Bitti. Hata yazisi gorduysen bu pencereyi bana goster.
pause
