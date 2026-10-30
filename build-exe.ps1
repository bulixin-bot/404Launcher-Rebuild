[CmdletBinding(DefaultParameterSetName = 'Archive')]
param(
    [Parameter(Mandatory = $true, ParameterSetName = 'Archive')][string]$OriginalArchive,
    [Parameter(Mandatory = $true, ParameterSetName = 'Directory')][string]$OriginalDirectory,
    [string]$PythonExe = 'python',
    [string]$OutputDirectory = '',
    [switch]$SkipRunCheck
)
$ErrorActionPreference = 'Stop'
$arguments = @((Join-Path $PSScriptRoot 'packaging/build_exe.py'))
if ($PSCmdlet.ParameterSetName -eq 'Directory') {
    $arguments += @('--runtime-dir', $OriginalDirectory)
} else {
    $arguments += @('--archive', $OriginalArchive)
}
if ($OutputDirectory) { $arguments += @('--output-dir', $OutputDirectory) }
if ($SkipRunCheck) { $arguments += '--skip-check' }
& $PythonExe @arguments
if ($LASTEXITCODE -ne 0) { throw 'EXE build failed' }
