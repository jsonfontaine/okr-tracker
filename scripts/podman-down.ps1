param(
  [switch]$WithSeq
)

<#
  .SYNOPSIS
    Para e remove os containers do OKR Tracker.

  .PARAMETER WithSeq
    Inclui o container do Seq na operacao de parada.

  .EXAMPLE
    .\scripts\podman-down.ps1
    .\scripts\podman-down.ps1 -WithSeq
#>

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

Write-Host "Parando containers..." -ForegroundColor Yellow

podman rm -f okr-tracker 2>$null | Out-Null
Write-Host "  okr-tracker parado." -ForegroundColor Green

if ($WithSeq) {
  podman rm -f okr-tracker-seq 2>$null | Out-Null
  Write-Host "  okr-tracker-seq parado." -ForegroundColor Green
}

Write-Host "Containers parados." -ForegroundColor Green
