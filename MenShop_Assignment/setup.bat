@echo off
echo ========================================
echo       SETUP LOCAL DEVELOPMENT
echo ========================================
echo.

REM Check if files are being tracked by git and create them if missing
git ls-files --error-unmatch appsettings.json >nul 2>&1
if errorlevel 1 (
    REM File is not tracked, need to create it
    if exist "appsettings.template.json" (
        copy "appsettings.template.json" "appsettings.json"
        echo ✓ Created appsettings.json from template
    ) else (
        echo Creating basic appsettings.json...
        echo { > appsettings.json
        echo   "ConnectionStrings": { >> appsettings.json
        echo     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDb;Trusted_Connection=true" >> appsettings.json
        echo   }, >> appsettings.json
        echo   "Logging": { >> appsettings.json
        echo     "LogLevel": { >> appsettings.json
        echo       "Default": "Information" >> appsettings.json
        echo     } >> appsettings.json
        echo   } >> appsettings.json
        echo } >> appsettings.json
        echo ✓ Created basic appsettings.json
    )
)

git ls-files --error-unmatch Properties\launchSettings.json >nul 2>&1
if errorlevel 1 (
    if exist "Properties\launchSettings.template.json" (
        copy "Properties\launchSettings.template.json" "Properties\launchSettings.json"
        echo ✓ Created launchSettings.json from template
    ) else (
        REM Create basic launchSettings.json
        if not exist "Properties" mkdir Properties
        echo Creating basic launchSettings.json...
        echo { > Properties\launchSettings.json
        echo   "profiles": { >> Properties\launchSettings.json
        echo     "YourProject": { >> Properties\launchSettings.json
        echo       "commandName": "Project", >> Properties\launchSettings.json
        echo       "launchBrowser": true, >> Properties\launchSettings.json
        echo       "applicationUrl": "https://localhost:7001;http://localhost:5000", >> Properties\launchSettings.json
        echo       "environmentVariables": { >> Properties\launchSettings.json
        echo         "ASPNETCORE_ENVIRONMENT": "Development" >> Properties\launchSettings.json
        echo       } >> Properties\launchSettings.json
        echo     } >> Properties\launchSettings.json
        echo   } >> Properties\launchSettings.json
        echo } >> Properties\launchSettings.json
        echo ✓ Created basic launchSettings.json
    )
)

REM Làm cho Git bỏ qua các thay đổi
echo.
echo Configuring Git to ignore local changes...
git update-index --assume-unchanged appsettings.json 2>nul
git update-index --assume-unchanged appsettings.Development.json 2>nul
git update-index --assume-unchanged Properties\launchSettings.json 2>nul

REM Kiểm tra nếu có thư mục Migrations
if exist Migrations (
    for /r Migrations %%f in (*) do (
        git update-index --assume-unchanged "%%f" 2>nul
    )
)

echo ✓ Git configured to ignore changes to config files
echo.
echo ✓ Setup completed! You can now safely modify config files.
echo   Your changes will not appear in Git status.
echo.
pause
