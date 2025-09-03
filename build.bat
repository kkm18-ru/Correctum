@echo off
chcp 65001

REM Использование:
REM build.bat (Номер версии) (Токен Telegram-бота) (ID группы/пользователя)

dotnet publish --sc -r win-x86 -o "%TEMP%\RetailCorrectorBuild" -c Release -p:Version=%1
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" /DAppVersion=%1 /DSourcePath=%TEMP%\RetailCorrectorBuild RetailCorrector.iss
dotnet clean

curl --request POST --url "https://api.telegram.org/bot%2/sendDocument" --header "content-type: multipart/form-data" --form "chat_id=%3" --form "document=@%cd%\RetailCorrector-%1.exe"
rmdir /Q /S "%TEMP%\RetailCorrectorBuild"
del /Q /S RetailCorrector-%1.exe