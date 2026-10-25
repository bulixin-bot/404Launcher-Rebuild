param([string]$OriginalRoot = 'E:\404Launcher1.3.8')
$ErrorActionPreference = 'Stop'
$root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$project = Join-Path $PSScriptRoot 'OfflineDifferential.csproj'
$output = Join-Path $root 'artifacts\offline-differential'
New-Item -ItemType Directory -Force $output | Out-Null
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
dotnet restore $project --configfile (Join-Path $PSScriptRoot 'NuGet.Config') *> (Join-Path $output 'restore.log')
if ($LASTEXITCODE -ne 0) { throw 'Offline restore failed; inspect restore.log' }
dotnet build $project --no-restore -c Release -warnaserror *> (Join-Path $output 'build.log')
if ($LASTEXITCODE -ne 0) { throw 'Build failed; inspect build.log' }
dotnet (Join-Path $PSScriptRoot 'bin\Release\net9.0\OfflineDifferential.dll') $root $OriginalRoot
if ($LASTEXITCODE -ne 0) { throw 'Differential execution failed; inspect artifacts' }
