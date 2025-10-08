# Railway Deployment Guide for CSV to VCF Converter

## Step 1: Prepare Your Project
1. Make sure your project builds successfully:
   ```bash
   dotnet build
   ```

## Step 2: Create railway.json (Optional but recommended)
Create this file in your project root for better deployment:

```json
{
  "$schema": "https://railway.app/railway.schema.json",
  "build": {
    "builder": "NIXPACKS"
  },
  "deploy": {
    "startCommand": "dotnet CsvToVcfConverter.dll",
    "healthcheckPath": "/",
    "healthcheckTimeout": 100
  }
}
```

## Step 3: Push to GitHub
1. Initialize git if not already done:
   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   ```

2. Create GitHub repository and push:
   ```bash
   git remote add origin https://github.com/yourusername/csv-to-vcf-converter.git
   git branch -M main
   git push -u origin main
   ```

## Step 4: Deploy to Railway
1. Go to https://railway.app
2. Sign up with GitHub account
3. Click "Start a New Project"
4. Select "Deploy from GitHub repo"
5. Choose your csv-to-vcf-converter repository
6. Railway will automatically:
   - Detect it's a .NET project
   - Build and deploy
   - Provide you with a live URL

## Step 5: Configure Environment (if needed)
- Railway automatically handles most .NET configurations
- Your app will be available at: `https://your-app-name.up.railway.app`

## Cost: FREE
- Railway gives $5 free credit monthly
- Your small app will likely use $1-2/month
- Perfect for personal projects and demos

## Domain (Optional)
- You can add a custom domain later in Railway dashboard
- SSL certificates are automatically provided