# CSV to VCF Converter

A static website that converts CSV and Excel contact files into vCard 3.0 (`.vcf`) files. No .NET, backend, database, build step, account, or API keys are required.

## Features

- CSV, XLSX, and XLS input, up to 20 MB
- Worksheet selection for Excel workbooks
- Automatic suggestions and manual selection for name and phone columns
- Preview and contact count; rows missing a name or phone are skipped
- Optional name prefix and suffix, applied to every exported contact with a live preview
- UTF-8 vCard downloads with escaped fields and folded lines
- Responsive layout and keyboard-accessible controls
- Files are read in browser memory, never uploaded or persisted by the site

Use a header row. Store phone numbers as **text** in Excel to preserve leading zeros and plus signs. The converter cannot recover digits or formatting already lost in the source file.

## Deploy to Vercel

1. Commit and push these changes to your GitHub repository.
2. In Vercel, choose **Add New → Project** and import this repository.
3. Set **Framework Preset: Other** and keep **Root Directory** at the repository root.
4. Leave **Build Command** empty and use **Output Directory: public**. The included `vercel.json` configures this automatically.
5. Deploy. No environment variables are needed.

Only `public/` is published. The original ASP.NET application remains in `CsvToVcfConverter/` for reference and is not used by the static website.

## Local preview

Open `public/index.html` directly in a modern browser, or serve `public/` with any static web server. For example, with Node.js:

```sh
npx --yes serve public
```

Node.js is optional for development tools; it is not required by the deployed website.

## Verification

With Node.js installed:

```sh
node --test tests/converter.test.cjs
node --check public/app.js
```

Tests cover CSV quoting and multiline fields, phone-number preservation, XLS/XLSX round trips, worksheet handling, invalid mappings, vCard escaping, and UTF-8 line folding.

## Files

- `public/index.html`: interface
- `public/styles.css`: responsive styling
- `public/app.js`: file selection, parsing, preview, and downloads
- `public/converter.js`: contact filtering and vCard generation
- `public/vendor/`: bundled SheetJS Community Edition 0.20.3 and its Apache 2.0 license
- `vercel.json`: static output and security headers

SheetJS is bundled locally from https://cdn.sheetjs.com/xlsx-0.20.3/package/dist/xlsx.full.min.js; the website does not depend on a third-party CDN at runtime. There are no analytics, cookies, or contact-data network requests. The hosting provider still receives ordinary website requests.

The older Docker, Railway, and .NET setup files apply only to the legacy application.
