@echo off
setlocal
cd /d "%~dp0"
set "PATH=%PATH%;C:\Program Files\Git\cmd"

echo === Git kontrol ===
git --version
if errorlevel 1 (
  echo Git bulunamadi. Git kurulumunu kontrol et.
  pause
  exit /b 1
)

git config --global user.name "Furkan Balpetek"
git config --global user.email "furkanbalpetekk@gmail.com"

if not exist ".git" git init
git add .
git commit -m "Ilk Pong oyunu"
git branch -M main
git remote remove origin >nul 2>&1
git remote add origin https://github.com/furkanbalpetek/Pong.git

echo === GitHub'a gonderiliyor (giris penceresi acilirsa onayla) ===
git push -u origin main

echo.
echo Bitti. Hata yazisi gorduysen bu pencereyi bana goster.
pause
