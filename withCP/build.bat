@echo off
set CSC=%WINDIR%\Microsoft.NET\Framework\v2.0.50727\csc.exe

if not exist "%CSC%" (
    echo Khong tim thay csc.exe. Hay kiem tra da bat .NET Framework 3.5 chua.
    pause
    exit /b 1
)

"%CSC%" /nologo /target:exe /out:BaselineLab.exe /reference:System.Data.dll Program.cs

if %ERRORLEVEL% EQU 0 (
    echo Build thanh cong: BaselineLab.exe
) else (
    echo Build that bai.
)
pause