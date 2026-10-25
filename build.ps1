param([ValidateSet('Debug', 'Release')][string]$Configuration = 'Release')
$ErrorActionPreference = 'Stop'
& dotnet build (Join-Path $PSScriptRoot '404Launcher.Rebuild.sln') -c $Configuration --nologo
if ($LASTEXITCODE -ne 0) { throw 'Backend build failed' }
