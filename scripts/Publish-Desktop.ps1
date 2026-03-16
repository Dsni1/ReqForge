param(
    [string]$Version = "0.1.0-local"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repoRoot "ReqForge.Desktop\ReqForge.Desktop.csproj"
$artifactsRoot = Join-Path $repoRoot "artifacts\desktop"

$rids = @("win-x64", "linux-x64")

if (-not (Test-Path $artifactsRoot)) {
    New-Item -ItemType Directory -Path $artifactsRoot | Out-Null
}

foreach ($rid in $rids) {
    $publishDir = Join-Path $artifactsRoot "$rid\publish"
    $packageBase = "ReqForge-$Version-$rid"

    if (Test-Path $publishDir) {
        Remove-Item -Recurse -Force $publishDir
    }

    New-Item -ItemType Directory -Path $publishDir | Out-Null

    dotnet publish $projectPath `
        -c Release `
        -f net8.0 `
        -r $rid `
        --self-contained true `
        /p:PublishSingleFile=true `
        /p:IncludeNativeLibrariesForSelfExtract=true `
        /p:PublishTrimmed=false `
        -o $publishDir

    if ($LASTEXITCODE -ne 0) {
        throw "Publish failed for runtime '$rid'."
    }

    if ($rid -like "win-*") {
        $zipPath = Join-Path $artifactsRoot "$packageBase.zip"
        if (Test-Path $zipPath) {
            Remove-Item -Force $zipPath
        }

        Compress-Archive -Path (Join-Path $publishDir "*") -DestinationPath $zipPath
        Write-Host "Created: $zipPath"
    }
    else {
        $tarPath = Join-Path $artifactsRoot "$packageBase.tar.gz"
        if (Test-Path $tarPath) {
            Remove-Item -Force $tarPath
        }

        tar -czf $tarPath -C $publishDir .
        if ($LASTEXITCODE -ne 0) {
            throw "Archive creation failed for runtime '$rid'."
        }

        Write-Host "Created: $tarPath"
    }
}

Write-Host "Done. Artifacts are in: $artifactsRoot"

