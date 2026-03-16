# ReqForge

ReqForge is a personal API request tester built with Avalonia and .NET 8.

## Local run (desktop)

```powershell
dotnet run --project .\ReqForge.Desktop\ReqForge.Desktop.csproj
```

## Publish for Windows and Linux

The repository includes a helper script that creates self-contained desktop builds for:

- `win-x64` (zip)
- `linux-x64` (tar.gz)

Run:

```powershell
.\scripts\Publish-Desktop.ps1 -Version 0.1.0
```

Artifacts are generated under `artifacts/desktop/`.

### Install on Windows

1. Extract `ReqForge-<version>-win-x64.zip`
2. Run `ReqForge.Desktop.exe`

### Install on Linux

1. Copy `ReqForge-<version>-linux-x64.tar.gz` to the Linux machine
2. Extract it
3. Make executable and run:

```bash
chmod +x ReqForge.Desktop
./ReqForge.Desktop
```

## Put project on GitHub

1. Create a new empty repository on GitHub (without README/.gitignore)
2. In this folder, run:

```powershell
git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin <YOUR_GITHUB_REPO_URL>
git push -u origin main
```

If you use GitHub CLI, you can create and push in one flow:

```powershell
gh repo create ReqForge --private --source . --remote origin --push
```

