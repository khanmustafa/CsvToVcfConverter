@echo off
echo Starting deployment preparation...

echo.
echo Step 1: Building the project...
cd CsvToVcfConverter
dotnet build
if %ERRORLEVEL% neq 0 (
    echo Build failed! Please fix errors and try again.
    pause
    exit /b 1
)

echo.
echo Step 2: Preparing git repository...
cd ..
git add .
git status

echo.
echo Step 3: Committing changes...
set /p commit_message="Enter commit message (or press Enter for default): "
if "%commit_message%"=="" set commit_message=Update CSV to VCF Converter

git commit -m "%commit_message%"

echo.
echo Step 4: Pushing to GitHub...
echo Make sure you have created a GitHub repository first!
echo Repository URL should be: https://github.com/yourusername/csv-to-vcf-converter.git
echo.
set /p push_confirm="Ready to push to GitHub? (y/n): "
if /i "%push_confirm%"=="y" (
    git push origin main
    echo.
    echo ✅ Successfully pushed to GitHub!
    echo.
    echo Next steps:
    echo 1. Go to https://railway.app
    echo 2. Sign in with GitHub
    echo 3. Click "Deploy from GitHub repo"
    echo 4. Select your csv-to-vcf-converter repository
    echo 5. Railway will automatically deploy your app!
    echo.
    echo Your app will be live at: https://your-app-name.up.railway.app
) else (
    echo Deployment cancelled.
)

echo.
pause