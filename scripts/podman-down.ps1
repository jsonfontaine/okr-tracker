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

Remove-Item Env:OKR_DB_HOST_PATH  -ErrorAction SilentlyContinue
Remove-Item Env:OKR_APP_PORT      -ErrorAction SilentlyContinue
Remove-Item Env:OKR_SEQ_PORT      -ErrorAction SilentlyContinue
Remove-Item Env:OKR_CERT_PASSWORD -ErrorAction SilentlyContinue

Set-Location $repoRoot

# --profile e uma flag global do compose: deve vir ANTES do subcomando
$downArgs = @("compose")
if ($WithSeq) {
  $downArgs += @("--profile", "observability")
}
$downArgs += "down"

Write-Host "Parando containers..." -ForegroundColor Yellow
podman @downArgs
Write-Host "Containers parados." -ForegroundColor Green

