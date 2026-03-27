<#
  .SYNOPSIS
    Mostra o status dos containers do OKR Tracker.

  .EXAMPLE
    .\scripts\podman-status.ps1
#>

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

Write-Host ""
Write-Host "=== Containers OKR Tracker ===" -ForegroundColor Cyan
podman ps --filter "name=okr-tracker" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
Write-Host ""

