#Requires -Version 5.1
<#
.SYNOPSIS
  Builds the VR mod, refreshes both distribution payloads, and produces the
  release artifacts in .\dist\.

.DESCRIPTION
  Outputs:
    dist\VSVR2-<version>.zip          - Manual install zip (mod-only overlay)
    dist\VSVRInstaller-<version>.exe  - Installer with the full payload embedded

  Payload files (BepInEx core, native plugins, asset bundles, etc.) come from
  the .\payload\ tree which is committed to the repo. Only VSVRMod2.dll and
  changelog.txt are rewritten on each run.

.PARAMETER InstallerRepo
  Path to the VSVRInstaller source folder. Defaults to .\Installer\VSVRInstaller
  within this repo.

.PARAMETER SkipInstaller
  Skip rebuilding the .exe installer; only rebuild the manual zip.
#>
param(
    [string]$InstallerRepo = (Join-Path $PSScriptRoot "Installer\VSVRInstaller"),
    [switch]$SkipInstaller
)

$ErrorActionPreference = "Stop"
$repoRoot      = $PSScriptRoot
$modCsproj     = Join-Path $repoRoot "VSVRMod2.csproj"
$manualPayload = Join-Path $repoRoot "payload\manual"
$installerPayload = Join-Path $repoRoot "payload\installer"
$distDir       = Join-Path $repoRoot "dist"

function Read-ModVersion {
    [xml]$xml = Get-Content -LiteralPath $modCsproj
    $v = $xml.Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1
    if (-not $v) { throw "Could not read <Version> from $modCsproj" }
    return [string]$v
}

function Invoke-Step($name, [scriptblock]$body) {
    Write-Host "==> $name" -ForegroundColor Cyan
    & $body
}

$version = Read-ModVersion
Write-Host "Mod version: $version" -ForegroundColor Green
New-Item -ItemType Directory -Force -Path $distDir | Out-Null

Invoke-Step "Building VSVRMod2 (Release)" {
    & dotnet build $modCsproj -c Release --nologo -v minimal
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed ($LASTEXITCODE)" }
}

$builtDll = Join-Path $repoRoot "bin\Release\netstandard2.1\VSVRMod2.dll"
if (-not (Test-Path -LiteralPath $builtDll)) {
    throw "Built dll not found at $builtDll"
}

Invoke-Step "Staging VSVRMod2.dll into payloads" {
    $manualDest    = Join-Path $manualPayload "VSVRMod2.dll"
    $installerDest = Join-Path $installerPayload "BepInEx\plugins\VSVRMod2.dll"
    New-Item -ItemType Directory -Force -Path (Split-Path $installerDest) | Out-Null
    Copy-Item -LiteralPath $builtDll -Destination $manualDest    -Force
    Copy-Item -LiteralPath $builtDll -Destination $installerDest -Force
}

function New-PayloadZip($sourceDir, $zipPath) {
    if (Test-Path -LiteralPath $zipPath) { Remove-Item -LiteralPath $zipPath -Force }
    # Compress-Archive on PS5.1 doesn't preserve empty dirs and can be slow on
    # large trees, so use ZipFile directly.
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory(
        $sourceDir, $zipPath,
        [System.IO.Compression.CompressionLevel]::Optimal,
        $false)
}

Invoke-Step "Packing manual-install zip" {
    $manualZip = Join-Path $distDir "VSVR2-$version.zip"
    New-PayloadZip $manualPayload $manualZip
    Write-Host "  -> $manualZip"
}

if ($SkipInstaller) {
    Write-Host "Skipping installer build (-SkipInstaller)." -ForegroundColor Yellow
    return
}

if (-not (Test-Path -LiteralPath $InstallerRepo)) {
    throw "Installer repo not found at $InstallerRepo. Pass -InstallerRepo <path> or use -SkipInstaller."
}

$installerCsproj = Join-Path $InstallerRepo "VSVRInstaller.csproj"
$embeddedZipDest = Join-Path $InstallerRepo "VSVRMOD.zip"

Invoke-Step "Packing installer payload into embedded resource" {
    New-PayloadZip $installerPayload $embeddedZipDest
    Write-Host "  -> $embeddedZipDest"
}

function Find-MSBuild {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path -LiteralPath $vswhere) {
        $path = & $vswhere -latest -requires Microsoft.Component.MSBuild `
            -find "MSBuild\**\Bin\MSBuild.exe" 2>$null | Select-Object -First 1
        if ($path -and (Test-Path -LiteralPath $path)) { return $path }
    }
    $cmd = Get-Command msbuild.exe -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }
    throw "Could not locate MSBuild.exe. Install Visual Studio Build Tools or open VS Developer PowerShell."
}

Invoke-Step "Building VSVRInstaller (Release, .NET Framework MSBuild)" {
    # VSVRInstaller targets .NET Framework 4.8 with legacy resgen tasks that
    # `dotnet build` (Core MSBuild) cannot host. Use full MSBuild from VS.
    $msbuild = Find-MSBuild
    & $msbuild $installerCsproj /nologo /v:minimal /p:Configuration=Release /p:Platform=AnyCPU /restore
    if ($LASTEXITCODE -ne 0) { throw "msbuild (VSVRInstaller) failed ($LASTEXITCODE)" }
}

$builtExe = Join-Path $InstallerRepo "bin\Release\VSVRInstaller.exe"
if (-not (Test-Path -LiteralPath $builtExe)) {
    throw "Built installer not found at $builtExe"
}

Invoke-Step "Copying installer to dist" {
    $distExe = Join-Path $distDir "VSVRInstaller-$version.exe"
    Copy-Item -LiteralPath $builtExe -Destination $distExe -Force
    Write-Host "  -> $distExe"
}

Write-Host ""
Write-Host "Done. Artifacts in $distDir" -ForegroundColor Green
Get-ChildItem -LiteralPath $distDir | Format-Table Name, Length, LastWriteTime
