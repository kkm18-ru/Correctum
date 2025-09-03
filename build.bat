@echo off
chcp 65001

IF "%1"=="" (
	set /P "vers=Номер версии сборки: "
) else (
	set "vers=%1"
)

dotnet publish --sc -r win-x86 -o TEMP/publish -c Release -p:Version=%vers%
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" /DAppVersion=%vers% RetailCorrector.iss
dotnet clean

IF "%2"=="" (
	set "localPath=\\192.168.2.89\share_egais\Distr\RetailCorrector"
) else (
	set "localPath=%2"
)

move /-Y TEMP\RetailCorrector-%vers%.exe %localPath%\RetailCorrector-%vers%.exe
rmdir /Q /S TEMP
