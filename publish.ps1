@echo off
REM Build single-file Ender Assist EXE for Windows x64
dotnet publish src\MarlinPrintMiddleware.App -c Release -r win-x64
echo.
echo Published: src\MarlinPrintMiddleware.App\bin\Release\net8.0-windows\win-x64\publish\EnderAssist.exe
